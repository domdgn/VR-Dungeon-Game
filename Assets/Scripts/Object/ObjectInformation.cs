using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// This class represents an object that has damageable properties, such as taking fall damage.
public class ObjectInformation : MonoBehaviour, IDamageable
{
    [SerializeField] private ScriptableObjectInformation ItemInformation; // ScriptableObject that stores object data (e.g., name, min/max value, etc.)
    public GameObject damageUIprefab; // Prefab used to visually display damage taken
    public Transform uiSpawnPoint; // Optional transform to determine where to spawn damage UI
    public float value; // Represents the object's value or "health"
    MeshRenderer meshRenderer; // Reference to the object's MeshRenderer to change material color when damaged
    Color origColor; // Stores the original material color for damage flash effect
    float flashTime = 0.15f; // Duration of the red flash effect when damaged
    public AudioSource audioSource; // Audio source to play damage sound

    private void Start()
    {
        // Log the object's name from the ScriptableObject for debugging
        Debug.Log(ItemInformation.objectName);

        // Assign a random initial value (acts like health) within the defined range
        Value();

        // Cache the MeshRenderer and store its original color
        meshRenderer = GetComponentInChildren<MeshRenderer>(true);
        origColor = meshRenderer.material.color;
    }

    // Assign a random value based on the min and max from the ScriptableObject
    void Value()
    {
        value = Random.Range(ItemInformation.minValue, ItemInformation.maxValue);
    }

    // Method required by the IDamageable interface to handle fall damage
    public void Damage(float impactVelocity)
    {
        // Only apply damage if fall speed exceeds safe threshold
        if (impactVelocity > ItemInformation.safeFallVelocity)
        {
            // Calculate damage using the velocity difference and multiplier
            float damage = (impactVelocity - ItemInformation.safeFallVelocity) * ItemInformation.damageMultiplier;

            // Subtract damage from the object's current value
            value -= damage;

            // Trigger visual and audio feedback
            StartCoroutine(Flash());
            ShowDamageUI(damage);
            DamageSound();

            // Log the damage taken and the new value
            Debug.Log($"Object took {damage} fall damage! Health is now {value}");

            // Destroy the object if its value drops below 1
            if (value < 1)
            {
                Destroy(gameObject);
                Debug.Log("Break Me!!");
            }
        }
        else
        {
            // Log that the object landed safely with no damage
            Debug.Log("Object landed safely. No damage taken.");
        }
    }

    // Coroutine that flashes the object red briefly to show it's been damaged
    IEnumerator Flash()
    {
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        meshRenderer.material.color = origColor;
    }

    // Spawns floating damage text UI above the object
    void ShowDamageUI(float damage)
    {
        if (damageUIprefab != null)
        {
            // Determine spawn position for UI
            Vector3 spawnPos = uiSpawnPoint ? uiSpawnPoint.position : transform.position + Vector3.up;

            // Instantiate the UI and set its text to the damage value
            GameObject ui = Instantiate(damageUIprefab, spawnPos, Quaternion.identity);
            ui.GetComponentInChildren<TextMeshProUGUI>().text = "-" + damage.ToString("0");
        }
    }

    // Plays a sound effect when the object takes damage
    void DamageSound()
    {
        // Ensure we have an AudioSource to play from
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Play the damage sound if both audio source and clip are set
        if (audioSource != null && ItemInformation.damageAudio != null)
        {
            audioSource.PlayOneShot(ItemInformation.damageAudio);
        }
    }

    // Returns the current value (or health) of the object
    public float GetValue()
    {
        return value;
    }
}
