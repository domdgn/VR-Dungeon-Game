using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralDungeon
{
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject startRoom;
        public GameObject masterPrefab;
        public GameObject closedDoorPrefab;

        [Header("Generation Settings")]
        [Tooltip("Each level is its own Scriptable Object and has its own generation settings and prefabs.\nRefer to the Demo Level SO for more info.")]
        public List<LevelSO> levels = new List<LevelSO>();

        private LevelSO levelData;
        private float hallwayChance;
        private float sequentialHallwayChanceMulti;

        private float currentHallwayChance;
        private List<GameObject> placedRooms = new List<GameObject>();
        private GameObject roomPrefabToPlace;
        private Transform dungeonParent;
        private int maxRooms;

        private void Awake()
        {
            placedRooms.Add(startRoom);
            ResetDungeon(); // Remove if you don't want dungeons to generate on Awake()
        }

        public void ClearLevel()
        {
            placedRooms.Clear();
            placedRooms.Add(startRoom);

            foreach (Transform child in startRoom.transform)
            {
                if (child.CompareTag("Entry") && child.GetComponent<EntryPoint>() && child.GetComponent<EntryPoint>().isConnected == true)
                {
                    child.GetComponent<EntryPoint>().isConnected = false;
                }
            }

            if (dungeonParent != null)
            {
                Destroy(dungeonParent.gameObject);
                dungeonParent = null;
                StopAllCoroutines();
            }
        }
        public void ResetDungeon()
        {
            ClearLevel();

            if (levels.Count > 0)
            {
                levelData = levels[Random.Range(0, levels.Count)];
                Debug.Log(levelData.levelName + "level selected");
            }

            SetUpLevelData(levelData);
            StartCoroutine(GenerateDungeon());
        }

        void SetUpLevelData(LevelSO levelData)
        {
            maxRooms = levelData.totalRooms;
            hallwayChance = levelData.hallwayChance;
            sequentialHallwayChanceMulti = levelData.sequentialHallwayChanceMultiplier;
            currentHallwayChance = hallwayChance;
        }

        IEnumerator GenerateDungeon()
        {
            if (dungeonParent == null)
            {
                GameObject parent = new GameObject("Generated Dungeon");
                dungeonParent = parent.transform;
            }

            while (placedRooms.Count < maxRooms)
            {
                yield return StartCoroutine(PlaceNextRoom());
            }

            SealOpenDoors();
        }


        IEnumerator PlaceNextRoom()
        {
            GameObject sourceRoom = placedRooms[Random.Range(0, placedRooms.Count)];

            if (levelData == null)
            {
                roomPrefabToPlace = masterPrefab;
            }

            else
            {
                if (Random.value < currentHallwayChance)
                {
                    roomPrefabToPlace = levelData.hallwayPrefabs[Random.Range(0, levelData.hallwayPrefabs.Count)];
                    currentHallwayChance = hallwayChance * sequentialHallwayChanceMulti;
                }
                else
                {
                    roomPrefabToPlace = levelData.roomPrefabs[Random.Range(0, levelData.roomPrefabs.Count)];
                    currentHallwayChance = hallwayChance;
                }
            }


            List<Transform> sourceEntries = new List<Transform>();
            List<Transform> newEntries = new List<Transform>();

            sourceEntries.Clear();
            newEntries.Clear();

            foreach (Transform child in sourceRoom.transform)
            {
                if (child.CompareTag("Entry") && child.GetComponent<EntryPoint>() && child.GetComponent<EntryPoint>().isConnected == false)
                {
                    sourceEntries.Add(child);
                }
            }

            if (sourceEntries.Count == 0)
            {
                placedRooms.Remove(sourceRoom);
                yield break;
            }

            Transform selectedSourceEntry = sourceEntries[Random.Range(0, sourceEntries.Count)];
            EntryPoint sourceEntryPoint = selectedSourceEntry.GetComponent<EntryPoint>();

            GameObject roomToSpawn = InstantiateUnderParent(roomPrefabToPlace, Vector3.zero, Quaternion.identity);

            foreach (Transform child in roomToSpawn.transform)
            {
                if (child.CompareTag("Entry") && child.GetComponent<EntryPoint>() && child.GetComponent<EntryPoint>().isConnected == false)
                {
                    newEntries.Add(child);
                }
            }

            Transform selectedNewEntry = newEntries[Random.Range(0, newEntries.Count)];
            EntryPoint newEntryPoint = selectedNewEntry.GetComponent<EntryPoint>();

            Quaternion rotation = Quaternion.FromToRotation(selectedNewEntry.forward, -selectedSourceEntry.forward);
            roomToSpawn.transform.rotation = rotation;

            FixUpsideDown(roomToSpawn);

            Vector3 entryGlobalPos = roomToSpawn.transform.TransformPoint(selectedNewEntry.localPosition);
            Vector3 offset = selectedSourceEntry.position - entryGlobalPos;
            roomToSpawn.transform.position = offset;

            if (IsRoomOverlapping(roomToSpawn))
            {
                Destroy(roomToSpawn);
                yield return null;
            }

            else
            {
                placedRooms.Add(roomToSpawn);
                sourceEntryPoint.isConnected = true;
                newEntryPoint.isConnected = true;

                yield return null;
            }
        }

        void FixUpsideDown(GameObject roomToSpawn)
        {
            if (Vector3.Dot(roomToSpawn.transform.up, Vector3.down) > 0.9f)
            {
                roomToSpawn.transform.Rotate(Vector3.forward, 180f, Space.Self);
            }
        }


        private bool IsRoomOverlapping(GameObject room)
        {
            Collider[] roomColliders = room.GetComponents<Collider>();

            LayerMask roomLayer = 1 << LayerMask.NameToLayer("Rooms");

            foreach (Collider collider in roomColliders)
            {
                Collider[] hitColliders = Physics.OverlapBox(room.transform.position, collider.bounds.extents, room.transform.rotation, roomLayer, QueryTriggerInteraction.Collide);

                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject != room)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        void SealOpenDoors()
        {
            foreach (GameObject room in placedRooms)
            {
                foreach (Transform child in room.transform)
                {
                    if (child.CompareTag("Entry") && child.GetComponent<EntryPoint>() && !child.GetComponent<EntryPoint>().isConnected)
                    {
                        bool hasOverlappingEntry = false;
                        float checkRadius = 0.1f;

                        Collider[] colliders = Physics.OverlapSphere(child.position, checkRadius, ~0, QueryTriggerInteraction.Collide);
                        foreach (Collider col in colliders)
                        {
                            if (col.transform == child) continue;

                            if (col.CompareTag("Entry") && col.GetComponent<EntryPoint>())
                            {
                                hasOverlappingEntry = true;
                                break;
                            }
                        }

                        if (!hasOverlappingEntry)
                        {
                            GameObject door = InstantiateUnderParent(closedDoorPrefab, child.transform.position, child.transform.rotation);
                        }
                    }
                }
            }
        }

        private GameObject InstantiateUnderParent(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject obj = Instantiate(prefab, position, rotation);
            obj.transform.SetParent(dungeonParent);
            return obj;
        }
    }
}

