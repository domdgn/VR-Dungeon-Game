using System.Collections.Generic;
using UnityEngine;

namespace ProceduralDungeon
{
    [CreateAssetMenu(fileName = "New Level Data", menuName = "DungeonGenerator/Level Data")]
    public class LevelSO : ScriptableObject
    {
        public string levelName;
        public List<GameObject> roomPrefabs = new List<GameObject>();
        public List<GameObject> hallwayPrefabs = new List<GameObject>();

        public List<GameObject> itemList = new List<GameObject>();

        [Header("Room Generation Settings")]
        [Tooltip("Total number of rooms, inclusive of hallways")]
        public int totalRooms = 15;

        [Range(0f, 1f)]
        [Tooltip("Percentage chance for a hallway to be generated instead of a room")]
        public float hallwayChance = 0.25f;

        [Range(0f, 1f)]
        [Tooltip("Percentage chance multiplier for a hallway to be generated after another hallway.\nDoesn't entirely prevent long hallways, as the source room is random.")]
        public float sequentialHallwayChanceMultiplier = 0.1f;
    }
}
