using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectInformation : MonoBehaviour, IDamageable
{
    [SerializeField] private ScriptableObjectInformation ItemInformation; // ScriptableObject that stores object data (e.g. name)
    public GameObject damageUIprefab;
    public Transform uiSpawnPoint;
    public float value; // value of object which will act as health too
    MeshRenderer meshRenderer;
    Color origColor;
    float flashTime = 0.15f; 
    public AudioSource audioSource;

    
    private void Start()
    {
        // Debug output to confirm values
        Debug.Log(ItemInformation.objectName);

        Value();

        meshRenderer = GetComponent<MeshRenderer>();
        origColor = meshRenderer.material.color;
    }

    void Value()
    {
        value = Random.Range(ItemInformation.minValue, ItemInformation.maxValue);
    }

    // This method is required by the IFallDamageable interface
    public void Damage(float impactVelocity)
    {
        // Only apply damage if falling faster than the safe velocity
        if (impactVelocity > ItemInformation.safeFallVelocity)
        {
            // Calculate how much damage to apply
            float damage = (impactVelocity - ItemInformation.safeFallVelocity) * ItemInformation.damageMultiplier;

            // Subtract damage from the object's "health"/"value"
            value -= damage;

            StartCoroutine(Flash());
            ShowDamageUI(damage);
            DamageSound();

            // Output the damage taken and remaining value
            Debug.Log($"Object took {damage} fall damage! Health is now {value}");
            if(value < 1)
            {
                //Destroy Object if Value is below 1 
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

    // This Coroutine is to add a visual for the damage 
    IEnumerator Flash()
    {
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        meshRenderer.material.color = origColor;
    }

    void ShowDamageUI(float damage)
    {
        if (damageUIprefab != null)
        {
            Vector3 spawnPos = uiSpawnPoint ? uiSpawnPoint.position : transform.position + Vector3.up;
            GameObject ui = Instantiate(damageUIprefab, spawnPos, Quaternion.identity);
            ui.GetComponentInChildren<TextMeshProUGUI>().text = "-" + damage.ToString("0");
        }
    }

     void DamageSound()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if (audioSource != null && ItemInformation.damageAudio != null)
        {
            audioSource.PlayOneShot(ItemInformation.damageAudio);
        }
    }


     public float GetValue()
    {
        return value;
    }
}