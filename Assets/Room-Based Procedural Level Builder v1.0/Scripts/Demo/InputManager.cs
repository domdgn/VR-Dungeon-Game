using UnityEngine;

namespace ProceduralDungeon
{
    public class InputManager : MonoBehaviour
    {
        public DungeonGenerator dungeonGenerator;

        private void Awake()
        {
            if (dungeonGenerator == null)
            {
                dungeonGenerator = FindObjectOfType<DungeonGenerator>();

                if (dungeonGenerator == null)
                {
                    Debug.LogWarning("No Dungeon Generator Found in scene!");
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && dungeonGenerator != null)
            {
                dungeonGenerator.ResetDungeon();
            }
        }
    }
}
