using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple frustum and occlusion culling script for XR applications
/// Supports runtime-instantiated objects
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

    [Header("Exclusions")]
    [Tooltip("Objects that should never be culled")]
    public List<GameObject> neverCullObjects = new List<GameObject>();

    // Internal variables
    private Camera _camera;
    private Plane[] _frustumPlanes = new Plane[6];
    private HashSet<Renderer> _trackedRenderers = new HashSet<Renderer>();
    private Dictionary<Renderer, GameObject> _rendererToGameObject = new Dictionary<Renderer, GameObject>();
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
                _exemptRenderers.Add(renderer);

                // Make sure exempt objects are visible
                if (renderer != null)
                {
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
            // Periodically update exempt renderers in case child objects change
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
        // Find all renderers in the cullable layer
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (((1 << obj.layer) & cullableLayer.value) != 0)
            {
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in renderers)
                {
                    if (!_trackedRenderers.Contains(renderer))
                    {
                        _trackedRenderers.Add(renderer);
                        _rendererToGameObject[renderer] = obj;
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

            // Get bounds and check distance
            Bounds bounds = renderer.bounds;
            float distance = Vector3.Distance(cameraPos, bounds.center);

            // Skip if too far away
            if (distance > maxCullDistance)
            {
                renderer.enabled = false;
                continue;
            }

            // Frustum culling
            bool inFrustum = GeometryUtility.TestPlanesAABB(_frustumPlanes, bounds);
            if (!inFrustum)
            {
                renderer.enabled = false;
                continue;
            }

            // Occlusion culling
            bool isVisible = !IsOccluded(cameraPos, bounds.center, _rendererToGameObject[renderer]);
            renderer.enabled = isVisible;
        }
    }

    private bool IsOccluded(Vector3 fromPos, Vector3 toPos, GameObject self)
    {
        Vector3 direction = toPos - fromPos;
        float distance = direction.magnitude;
        direction.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(fromPos, direction, out hit, distance, occluderLayer))
        {
            // Not occluded if we hit ourselves
            if (hit.collider.gameObject == self || hit.collider.transform.IsChildOf(self.transform))
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
                _rendererToGameObject.Remove(renderer);
            }
        }

        foreach (Renderer renderer in toRemove)
        {
            _trackedRenderers.Remove(renderer);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw culling radius in Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxCullDistance);
    }
}