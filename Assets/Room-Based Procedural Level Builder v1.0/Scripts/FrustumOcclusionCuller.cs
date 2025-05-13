using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple frustum and occlusion culling script for XR applications
/// Supports runtime-instantiated objects with improved room culling
/// </summary>
public class SimpleXRCulling : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Layer mask for objects to be culled")]
    public LayerMask cullableLayer;

    [Tooltip("Layer mask for occluder objects")]
    public LayerMask occluderLayer;

    [Tooltip("How often to refresh object list (seconds)")]
    public float objectListUpdateInterval = 1.0f;

    [Tooltip("How often to update culling (seconds)")]
    public float cullingUpdateInterval = 0.1f;

    [Tooltip("Maximum distance for culling (meters)")]
    public float maxCullDistance = 50f;

    [Tooltip("Minimum distance before occlusion culling activates")]
    public float minOcclusionDistance = 5f;

    [Tooltip("Number of rays to cast for occlusion check")]
    [Range(1, 5)]
    public int occlusionRayCount = 3;

    [Header("Exclusions")]
    [Tooltip("Objects that should never be culled")]
    public List<GameObject> neverCullObjects = new List<GameObject>();

    // Internal variables
    private Camera _camera;
    private Plane[] _frustumPlanes = new Plane[6];
    private HashSet<Renderer> _trackedRenderers = new HashSet<Renderer>();
    private Dictionary<Renderer, GameObject> _rendererToGameObject = new Dictionary<Renderer, GameObject>();
    private Dictionary<Renderer, bool> _lastVisibilityState = new Dictionary<Renderer, bool>();
    private HashSet<Renderer> _exemptRenderers = new HashSet<Renderer>();
    private float _objectListTimer = 0f;
    private float _cullingTimer = 0f;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null)
        {
            Debug.LogError("SimpleXRCulling requires a Camera component!");
            enabled = false;
            return;
        }

        // Initial object scan
        FindCullableObjects();

        // Add exempt renderers from the never-cull list
        UpdateExemptRenderers();
    }

    /// <summary>
    /// Add a GameObject to the never-cull list at runtime
    /// </summary>
    public void AddNeverCullObject(GameObject obj)
    {
        if (obj != null && !neverCullObjects.Contains(obj))
        {
            neverCullObjects.Add(obj);
            UpdateExemptRenderers();
        }
    }

    /// <summary>
    /// Remove a GameObject from the never-cull list at runtime
    /// </summary>
    public void RemoveNeverCullObject(GameObject obj)
    {
        if (obj != null && neverCullObjects.Contains(obj))
        {
            neverCullObjects.Remove(obj);
            UpdateExemptRenderers();
        }
    }

    private void UpdateExemptRenderers()
    {
        _exemptRenderers.Clear();

        foreach (GameObject obj in neverCullObjects)
        {
            if (obj == null) continue;

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                {
                    _exemptRenderers.Add(renderer);
                    renderer.enabled = true;
                }
            }
        }
    }

    private void Update()
    {
        // Update object list periodically to catch runtime instantiated objects
        _objectListTimer += Time.deltaTime;
        if (_objectListTimer >= objectListUpdateInterval)
        {
            _objectListTimer = 0f;
            FindCullableObjects();
            CleanupTrackedRenderers();
            UpdateExemptRenderers();
        }

        // Update culling
        _cullingTimer += Time.deltaTime;
        if (_cullingTimer >= cullingUpdateInterval)
        {
            _cullingTimer = 0f;
            UpdateCulling();
        }
    }

    private void FindCullableObjects()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (((1 << obj.layer) & cullableLayer.value) != 0)
            {
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in renderers)
                {
                    if (renderer != null && !_trackedRenderers.Contains(renderer))
                    {
                        _trackedRenderers.Add(renderer);
                        _rendererToGameObject[renderer] = obj;
                        _lastVisibilityState[renderer] = true; // Default to visible
                    }
                }
            }
        }
    }

    private void UpdateCulling()
    {
        // Update frustum planes
        GeometryUtility.CalculateFrustumPlanes(_camera, _frustumPlanes);
        Vector3 cameraPos = _camera.transform.position;

        foreach (Renderer renderer in _trackedRenderers)
        {
            if (renderer == null) continue;

            // Skip renderers that should never be culled
            if (_exemptRenderers.Contains(renderer))
            {
                renderer.enabled = true;
                continue;
            }

            // Get bounds
            Bounds bounds = renderer.bounds;
            float distance = Vector3.Distance(cameraPos, bounds.center);

            // Skip if too far away - basic distance culling
            if (distance > maxCullDistance)
            {
                SetRendererState(renderer, false);
                continue;
            }

            // Frustum culling
            bool inFrustum = GeometryUtility.TestPlanesAABB(_frustumPlanes, bounds);
            if (!inFrustum)
            {
                // Only disable if it's a significant distance away
                if (distance > minOcclusionDistance)
                {
                    SetRendererState(renderer, false);
                }
                continue;
            }

            // Only do occlusion culling if beyond minimum distance
            // This prevents adjacent rooms from disappearing
            if (distance <= minOcclusionDistance)
            {
                SetRendererState(renderer, true);
                continue;
            }

            // Multi-ray occlusion culling for better accuracy
            bool isVisible = !IsOccludedMultiRay(cameraPos, bounds, _rendererToGameObject[renderer]);
            SetRendererState(renderer, isVisible);
        }
    }

    private void SetRendererState(Renderer renderer, bool visible)
    {
        // Check if state changed
        if (_lastVisibilityState.ContainsKey(renderer) && _lastVisibilityState[renderer] == visible)
        {
            // State hasn't changed, don't update
            return;
        }

        renderer.enabled = visible;
        _lastVisibilityState[renderer] = visible;
    }

    private bool IsOccludedMultiRay(Vector3 fromPos, Bounds targetBounds, GameObject self)
    {
        // Central ray to the center of the bounds
        bool centerOccluded = IsRayOccluded(fromPos, targetBounds.center, self);

        // If using just one ray, return the result of the center ray check
        if (occlusionRayCount <= 1) return centerOccluded;

        // Test additional rays if more than 1 ray requested
        int occludedCount = centerOccluded ? 1 : 0;
        float occlusionThreshold = Mathf.Ceil(occlusionRayCount * 0.6f); // 60% of rays must be occluded

        // Check different points on the bounds
        if (occlusionRayCount >= 3)
        {
            Vector3 topPoint = targetBounds.center + new Vector3(0, targetBounds.extents.y * 0.8f, 0);
            if (IsRayOccluded(fromPos, topPoint, self)) occludedCount++;

            Vector3 bottomPoint = targetBounds.center + new Vector3(0, -targetBounds.extents.y * 0.8f, 0);
            if (IsRayOccluded(fromPos, bottomPoint, self)) occludedCount++;
        }

        if (occlusionRayCount >= 5)
        {
            Vector3 leftPoint = targetBounds.center + new Vector3(-targetBounds.extents.x * 0.8f, 0, 0);
            if (IsRayOccluded(fromPos, leftPoint, self)) occludedCount++;

            Vector3 rightPoint = targetBounds.center + new Vector3(targetBounds.extents.x * 0.8f, 0, 0);
            if (IsRayOccluded(fromPos, rightPoint, self)) occludedCount++;
        }

        return occludedCount >= occlusionThreshold;
    }

    private bool IsRayOccluded(Vector3 fromPos, Vector3 toPos, GameObject self)
    {
        Vector3 direction = toPos - fromPos;
        float distance = direction.magnitude;
        direction.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(fromPos, direction, out hit, distance, occluderLayer))
        {
            // Not occluded if we hit ourselves
            if (hit.collider.gameObject == self ||
                (hit.collider.transform.IsChildOf(self.transform)) ||
                (self.transform.IsChildOf(hit.collider.transform)))
            {
                return false;
            }
            return true; // Occluded by something else
        }

        return false; // Nothing blocking the view
    }

    // Remove stale renderers
    private void CleanupTrackedRenderers()
    {
        List<Renderer> toRemove = new List<Renderer>();
        foreach (Renderer renderer in _trackedRenderers)
        {
            if (renderer == null)
            {
                toRemove.Add(renderer);
            }
        }

        foreach (Renderer renderer in toRemove)
        {
            _trackedRenderers.Remove(renderer);
            _rendererToGameObject.Remove(renderer);
            _lastVisibilityState.Remove(renderer);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw culling radius in Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxCullDistance);

        // Draw min occlusion distance
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, minOcclusionDistance);
    }
}