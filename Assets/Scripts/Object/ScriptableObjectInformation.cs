using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableItems", menuName = "ScriptableObject/ScriptableItems")]

public class ScriptableObjectInformation : ScriptableObject
{
   public string objectName; // Objects Name 
   public float mass; // weight of the object
   public AudioClip damageAudio;
   public float safeFallVelocity; // Hieght for the item to take damage
   public float damageMultiplier; // Set the multiplier for damage  
   public float minValue;
   public float maxValue;
}
