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

                bool shouldBeVisible = withinDistance && isVisible;

                //foreach (var r in renderers)
                //{
                //    if (r.enabled != shouldBeVisible)
                //        r.enabled = shouldBeVisible;
                //}

                var lights = obj.GetComponentsInChildren<Light>(true);
                foreach (var l in lights)
                {
                    if (l.gameObject.activeSelf != shouldBeVisible)
                        l.gameObject.SetActive(shouldBeVisible);
                }

                var colliders = obj.GetComponentsInChildren<MeshCollider>(true);
                foreach (var c in colliders)
                {
                    if (c.enabled != shouldBeVisible)
                        c.enabled = shouldBeVisible;
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
