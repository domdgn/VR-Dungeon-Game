using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInformation : MonoBehaviour, IDamageable
{
    [SerializeField] private ScriptableObjectInformation ItemInformation; // ScriptableObject that stores object data (e.g. name)
    public float value; // value of object which will act as health too
    MeshRenderer meshRenderer;
    Color origColor;
    float flashTime = 0.15f; 
    
    private void Start()
    {
        // Initialize object with randomized values from the ScriptableObject
        ItemInformation.Randomise();
        ItemInformation.Createprefab();
        // Debug output to confirm values
        Debug.Log(ItemInformation.objectName);

        Value();

        meshRenderer = GetComponent<MeshRenderer>();
        origColor = meshRenderer.material.color;
    }

    void Value()
    {
        value = Random.Range(40, 1000);
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
            value -= damage;

            StartCoroutine(Flash());

            // Output the damage taken and remaining value
            Debug.Log($"Object took {damage} fall damage! Health is now {value}");
            if(value < 1)
            {
                Destroy(gameObject);
                Debug.Log("Break Me!!");
            }
        }
        else
        {
            // If fall wasn't hard enough, no damage is taken
            Debug.Log("Object landed safely. No damage taken.");
        }
    }

    IEnumerator Flash()
    {
        meshRenderer.material.color = Color.green;
        yield return new WaitForSeconds(flashTime);
        meshRenderer.material.color = origColor;
    }

     public float GetValue()
    {
        return value;
    }
}