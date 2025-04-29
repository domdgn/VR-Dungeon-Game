using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProceduralDungeon
{
    public class demoGenerationScript : MonoBehaviour
    {
        private DungeonGenerator genScript;
        public InputActionReference genButton = null;

        void Start()
        {
            genScript = GetComponent<DungeonGenerator>();
        }

        private void Update()
        {
            float button = genButton.action.ReadValue<float>();
            if (button > 0)
            {
                genScript.ResetDungeon();
            }
        }
    }
}
