using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "objectInformation", menuName = "Custom/objectInformation")]

public class NewBehaviourScript : ScriptableObject
{
   public string objectName; // Objects Name 
   public float health; // Objects Health 
   public float mass; // weight of the object
   public float value; // value of object
   public GameObject ObjectPrefab; // Prefab for the objcet

    public void Randomise()
   {
      health = Random.Range(20, 100);
      mass = Random.Range(20, 100);
      value = Random.Range(40, 1000);
   }
}
