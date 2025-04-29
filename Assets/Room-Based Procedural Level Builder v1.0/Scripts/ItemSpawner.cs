using ProceduralDungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    private List<GameObject> items = new List<GameObject>();
    private List<Transform> itemSpawnPoints = new List<Transform>();

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
        if (Random.Range(0, 2) != 0)
        {
            return;
        }

        else
        {
            GameObject itemToSpawn = items[Random.Range(0, items.Count)];
            Debug.Log($"Spawning {itemToSpawn.name}");

            Instantiate(itemToSpawn, spawnPoint.position, spawnPoint.rotation);
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
