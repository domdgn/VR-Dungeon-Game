using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Static class that tracks dynamically instantiated renderers
public static class RendererTracker
{
    private static List<Renderer> _newRenderers = new List<Renderer>();

    // Call this when instantiating new objects with renderers
    public static void RegisterRenderer(Renderer renderer)
    {
        if (renderer != null && !_newRenderers.Contains(renderer))
        {
            _newRenderers.Add(renderer);
        }
    }

    // Get and clear the list of new renderers
    public static List<Renderer> GetAndClearNewRenderers()
    {
        List<Renderer> result = new List<Renderer>(_newRenderers);
        _newRenderers.Clear();
        return result;
    }
}

[RequireComponent(typeof(Camera))]
public class FrustumOcclusionCuller : MonoBehaviour
{
    // Layer mask for static occluders
    [SerializeField] private LayerMask occluderLayerMask = -1;

    // How many rays to cast for each object (more rays = more accurate but more expensive)
    [SerializeField] private int raysPerObject = 5;

    // The camera reference
    private Camera _camera;

    // Track all renderers in the scene
    private List<Renderer> _allRenderers = new List<Renderer>();

    // Frustum planes
    private Plane[] _frustumPlanes = new Plane[6];

    // Cache for bounds corners
    private Vector3[] _cornersCache = new Vector3[8];

    void Start()
    {
        _camera = GetComponent<Camera>();
        RefreshRenderersList();
    }

    // Call this method when new objects are instantiated
    public void RefreshRenderersList()
    {
        // Remove null renderers (destroyed objects)
        _allRenderers.RemoveAll(r => r == null);

        // Find all renderers in the scene
        Renderer[] sceneRenderers = FindObjectsOfType<Renderer>();
        foreach (Renderer renderer in sceneRenderers)
        {
            // Skip if already in our list or if it's a static occluder
            if (_allRenderers.Contains(renderer) ||
                ((1 << renderer.gameObject.layer) & occluderLayerMask.value) != 0)
                continue;

            _allRenderers.Add(renderer);
        }

        // Add any new renderers registered through the static tracker
        List<Renderer> newRenderers = RendererTracker.GetAndClearNewRenderers();
        foreach (Renderer renderer in newRenderers)
        {
            // Skip static occluders
            if (renderer == null ||
                _allRenderers.Contains(renderer) ||
                ((1 << renderer.gameObject.layer) & occluderLayerMask.value) != 0)
                continue;

            _allRenderers.Add(renderer);
        }
    }

    // Update frequency for finding new renderers (every X seconds)
    [SerializeField] private float dynamicUpdateInterval = 0.5f;
    private float _nextDynamicUpdate = 0f;

    void Update()
    {
        // Periodically check for new renderers
        if (Time.time > _nextDynamicUpdate)
        {
            RefreshRenderersList();
            _nextDynamicUpdate = Time.time + dynamicUpdateInterval;
        }

        // Calculate the camera's frustum planes
        _frustumPlanes = GeometryUtility.CalculateFrustumPlanes(_camera);

        foreach (Renderer renderer in _allRenderers)
        {
            if (renderer == null)
                continue;

            // First check if the object is within the frustum
            Bounds bounds = renderer.bounds;
            if (!GeometryUtility.TestPlanesAABB(_frustumPlanes, bounds))
            {
                renderer.enabled = false;
                continue;
            }

            // Object is in the frustum, now check if it's occluded
            if (IsOccluded(bounds))
            {
                renderer.enabled = false;
            }
            else
            {
                renderer.enabled = true;
            }
        }
    }

    private bool IsOccluded(Bounds bounds)
    {
        // Get the camera position
        Vector3 cameraPos = _camera.transform.position;

        // Get the bounds center and corners
        Vector3 center = bounds.center;
        GetBoundsCorners(bounds, _cornersCache);

        // Check if the center is occluded
        if (!IsPointOccluded(center, cameraPos))
            return false;

        // Use center + random points on the bounds for additional checks
        for (int i = 0; i < raysPerObject - 1; i++)
        {
            // Get a random point within the bounds
            Vector3 randomPoint;
            if (i < 8) // First use corners for better coverage
                randomPoint = _cornersCache[i];
            else
                randomPoint = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                );

            if (!IsPointOccluded(randomPoint, cameraPos))
                return false;
        }

        // All rays were occluded
        return true;
    }

    private bool IsPointOccluded(Vector3 point, Vector3 cameraPos)
    {
        // Direction from camera to point
        Vector3 direction = point - cameraPos;
        float distance = direction.magnitude;

        // Cast a ray from the camera to the point
        Ray ray = new Ray(cameraPos, direction.normalized);

        // Check if the ray hits anything in the occluder layer before reaching the point
        if (Physics.Raycast(ray, out RaycastHit hit, distance, occluderLayerMask))
        {
            // Something is blocking the view
            return true;
        }

        // Point is visible
        return false;
    }

    private void GetBoundsCorners(Bounds bounds, Vector3[] corners)
    {
        // Bottom face
        corners[0] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
        corners[1] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        corners[2] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
        corners[3] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);

        // Top face
        corners[4] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        corners[5] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        corners[6] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
        corners[7] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
    }

    // Visualize the frustum and rays in the editor for debugging
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || _camera == null)
            return;

        Gizmos.color = Color.green;

        // Draw frustum
        Gizmos.DrawRay(_camera.transform.position, _camera.transform.forward * _camera.farClipPlane);

        // Draw some test rays
        if (_allRenderers.Count > 0 && _allRenderers[0] != null)
        {
            Gizmos.color = Color.red;
            Bounds bounds = _allRenderers[0].bounds;
            Vector3 center = bounds.center;
            Gizmos.DrawLine(_camera.transform.position, center);
        }
    }
}