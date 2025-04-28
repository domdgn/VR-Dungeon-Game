using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralDungeon
{
    public class demoGenerationScript : MonoBehaviour
    {
        private DungeonGenerator genScript;

        void Start()
        {
            genScript = GetComponent<DungeonGenerator>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                genScript.ResetDungeon();
            }
        }
    }
}
