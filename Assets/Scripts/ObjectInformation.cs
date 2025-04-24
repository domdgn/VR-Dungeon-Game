using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInformation : MonoBehaviour, IDamageable
{
    [SerializeField] private ScriptableObjectInformation ItemInformation; // ScriptableObject that stores object data (e.g. name, health)

    private void Start()
    {
        // Initialize object with randomized values from the ScriptableObject
        ItemInformation.Randomise();
        ItemInformation.Createprefab();

        // Debug output to confirm values
        Debug.Log(ItemInformation.objectName);
        Debug.Log(ItemInformation.value);
    }

    // This method is required by the IFallDamageable interface
    public void Damage(float impactVelocity)
    {
        // Only apply damage if falling faster than the safe velocity
        if (impactVelocity > ItemInformation.safeFallVelocity)
        {
            // Calculate how much damage to apply
            float damage = (impactVelocity - ItemInformation.safeFallVelocity) * ItemInformation.damageMultiplier;

            // Subtract damage from the object's "health" or "value"
            ItemInformation.value -= damage;

            // Output the damage taken and remaining value
            Debug.Log($"Object took {damage} fall damage! Health is now {ItemInformation.value}");
        }
        else
        {
            // If fall wasn't hard enough, no damage is taken
            Debug.Log("Object landed safely. No damage taken.");
        }
    }
}