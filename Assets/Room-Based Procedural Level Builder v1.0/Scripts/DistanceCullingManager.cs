using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCullingManager : MonoBehaviour
{
    public static DistanceCullingManager Instance { get; private set; }
    public Transform player;
    public List<CullableObject> cullableObjects = new List<CullableObject>();
    public float checkInterval = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (player == null)
        {
            player = Camera.main.transform;
        }
        StartCoroutine(CheckDistances());
    }

    IEnumerator CheckDistances()
    {
        while (true)
        {
            Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            foreach (var obj in cullableObjects)
            {
                if (obj == null) continue;

                float distance = Vector3.Distance(obj.transform.position, player.position);
                bool withinDistance = distance < obj.activationDistance;

                var renderers = obj.GetComponentsInChildren<Renderer>(true);
                bool isVisible = false;

                foreach (var r in renderers)
                {
                    if (r != null && GeometryUtility.TestPlanesAABB(camPlanes, r.bounds))
                    {
                        isVisible = true;
                        break;
                    }
                }

                // Renderers should be visible if they're in view of the camera
                foreach (var r in renderers)
                {
                    if (r != null)
                    {
                        bool shouldRender = isVisible;
                        if (r.enabled != shouldRender)
                            r.enabled = shouldRender;
                    }
                }

                // Lights and colliders still obey the distance rule for performance
                bool shouldEnablePhysics = withinDistance;

                var lights = obj.GetComponentsInChildren<Light>(true);
                foreach (var l in lights)
                {
                    if (l != null && l.gameObject.activeSelf != shouldEnablePhysics)
                        l.gameObject.SetActive(shouldEnablePhysics);
                }

                var colliders = obj.GetComponentsInChildren<MeshCollider>(true);
                foreach (var c in colliders)
                {
                    if (c != null && c.enabled != shouldEnablePhysics && c.gameObject.CompareTag("Wall"))
                        c.enabled = shouldEnablePhysics;
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    public void AddToList(CullableObject self)
    {
        if (!cullableObjects.Contains(self))
        {
            cullableObjects.Add(self);
        }
    }
}