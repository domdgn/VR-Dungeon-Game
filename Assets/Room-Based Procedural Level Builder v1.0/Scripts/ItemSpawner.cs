using ProceduralDungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    private List<GameObject> items = new List<GameObject>();
    private List<Transform> itemSpawnPoints = new List<Transform>();
    public Vector3 spawnOffset = Vector3.zero;

    public void SetList(List<GameObject> itemList)
    {
        items = itemList;
    }

    public void AddItemSpawnPoints(GameObject roomToSpawn)
    {
        foreach (Transform child in roomToSpawn.transform)
        {
            if (child.CompareTag("ItemSpawn"))
            {
                itemSpawnPoints.Add(child);
            }
        }
    }

    private void SpawnItemAtPoint(Transform spawnPoint)
    {
        if (Random.Range(0, 3) != 0)
        {
            return;
        }

        else
        {
            GameObject itemToSpawn = items[Random.Range(0, items.Count)];
            Debug.Log($"Spawning {itemToSpawn.name}");

            Vector3 spawnPos = spawnPoint.position + spawnOffset;

            Instantiate(itemToSpawn, spawnPos, spawnPoint.rotation);
        }
    }

    public void TriggerItemSpawning()
    {
        Debug.Log("ItemSpawnTriggered");

        List<Transform> spawnPointsCopy = new List<Transform>(itemSpawnPoints);

        foreach (Transform transform in spawnPointsCopy)
        {
            Debug.Log("Attempting item spawn");
            SpawnItemAtPoint(transform);
            itemSpawnPoints.Remove(transform);
            Destroy(transform.gameObject);
        }

        itemSpawnPoints.Clear();
    }

}
