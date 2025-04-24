using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableItems", menuName = "ScriptableObject/ScriptableItems")]

public class ScriptableObjectInformation : ScriptableObject
{
   public string objectName; // Objects Name 
   public float mass; // weight of the object
   public float value; // value of object which will act as health too
   public float safeFallVelocity; // Hieght for the item to take damage
   public float damageMultiplier = 2; // Set the multiplier for damage  
   public GameObject ObjectPrefab; // Prefab for the objcet

   public void Randomise() // used to randomise the values of the items at the begining of the level
   {
      mass = Random.Range(20, 100);
      value = Random.Range(40, 1000);
      safeFallVelocity = Random.Range(1,10);
   }

   public void Createprefab()
   {
      Debug.Log("Hello"); // creates the prefab into the scene
   }
}
