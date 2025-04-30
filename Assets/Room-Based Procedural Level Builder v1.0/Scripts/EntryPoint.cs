using UnityEngine;

namespace ProceduralDungeon
{
    public class EntryPoint : MonoBehaviour
    {
        // ENSURE LOCAL Z-AXIS POINTS OUTWARD FOR EACH ENTRY POINT

        public bool isConnected = false;
        public EntryPoint connectedEntry;
        public bool doorAble = true;
    }
}
