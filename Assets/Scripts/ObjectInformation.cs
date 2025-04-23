using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInformation : MonoBehaviour
{
    [SerializeField] private ScriptableObjectInformation ItemInformation;

    private void Start() {
        Debug.Log(ItemInformation.objectName);
        Debug.Log (ItemInformation.health);
        ItemInformation.Randomise();
    }
}