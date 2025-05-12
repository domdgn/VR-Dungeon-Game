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
            foreach (var obj in cullableObjects)
            {
                if (obj == null) continue;

                float distance = Vector3.Distance(obj.transform.position, player.position);
                bool withinDistance = distance < obj.activationDistance;

                bool shouldEnableComponents = withinDistance;

                var lights = obj.GetComponentsInChildren<Light>(true);
                foreach (var l in lights)
                {
                    if (l != null && l.gameObject.activeSelf != shouldEnableComponents)
                        l.gameObject.SetActive(shouldEnableComponents);
                }

                var colliders = obj.GetComponentsInChildren<BoxCollider>(true);
                foreach (var c in colliders)
                {
                    if (c != null && c.enabled != shouldEnableComponents && c.gameObject.CompareTag("Wall"))
                        c.enabled = shouldEnableComponents;
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