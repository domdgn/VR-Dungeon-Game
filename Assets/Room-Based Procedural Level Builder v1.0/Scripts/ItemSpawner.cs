using ProceduralDungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnedItemsParent;
    [SerializeField] private Vector3 colliderBottomOffset = Vector3.zero;

    private List<GameObject> items = new List<GameObject>();
    private List<Transform> itemSpawnPoints = new List<Transform>();
    public Vector3 spawnOffset = Vector3.zero;

    private List<GameObject> spawnedItems = new List<GameObject>();

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
            return;

        GameObject itemToSpawn = items[Random.Range(0, items.Count)];
        //Debug.Log($"Spawning {itemToSpawn.name}");

        // Temporarily spawn the item at spawn point
        Vector3 spawnPos = spawnPoint.position + spawnOffset;
        GameObject spawnedItem = Instantiate(itemToSpawn, spawnPos, spawnPoint.rotation);

        Collider itemCollider = spawnedItem.GetComponent<Collider>();
        if (itemCollider != null)
        {
            float bottomOffset = itemCollider.bounds.min.y - spawnedItem.transform.position.y;

            Vector3 adjustedPosition = spawnPos;
            adjustedPosition.y -= bottomOffset;

            adjustedPosition += colliderBottomOffset;

            spawnedItem.transform.position = adjustedPosition;
        }


        if (spawnedItemsParent != null)
            spawnedItem.transform.SetParent(spawnedItemsParent);

        spawnedItems.Add(spawnedItem);
    }

    public IEnumerator TriggerItemSpawning()
    {
        Debug.Log("ItemSpawnTriggered");
        List<Transform> spawnPointsCopy = new List<Transform>(itemSpawnPoints);
        foreach (Transform transform in spawnPointsCopy)
        {
            Debug.Log("Attempting item spawn");
            SpawnItemAtPoint(transform);
            itemSpawnPoints.Remove(transform);
            Destroy(transform.gameObject);
            yield return null;
        }
        itemSpawnPoints.Clear();
        yield return null;
    }


    public void DeleteAllItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item.gameObject);
        }
        spawnedItems.Clear();
    }

    public void RemoveItemFromList(GameObject item)
    {
        spawnedItems.Remove(item);
        Debug.Log($"{item.name} removed from spawned items list.");
    }

    public void AddItemToList(GameObject item)
    {
        spawnedItems.Add(item);
        Debug.Log($"{item.name} added to spawned items list.");
    }
}
