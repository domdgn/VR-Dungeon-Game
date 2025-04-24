using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInformation : MonoBehaviour, IDamageable
{
    [SerializeField] private ScriptableObjectInformation ItemInformation;

    public float damageMultiplier = 2f;

    private Rigidbody rb;
    private bool isGrounded = false;
    private float lastYVelocity = 0f;

    private void Start() 
    {
        ItemInformation.Randomise(); // starts the function in the ScriptableObjectInformation file
        ItemInformation.Createprefab(); // starts the function in the ScriptableObjectInformation file
        Debug.Log(ItemInformation.objectName); // debug to check the scriptable object works 
        Debug.Log (ItemInformation.value); // debug to check the randomiser works 

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isGrounded)
        {
            lastYVelocity = rb.velocity.y;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded && collision.gameObject.CompareTag("Ground"))
        {
            float impactVelocity = Mathf.Abs(lastYVelocity);
            Damage(impactVelocity);
            isGrounded = true;
        }
    }

    void OnCollisionExit (Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void Damage(float impactVelocity)
    {
       if (impactVelocity > ItemInformation.safeFallVelocity)
       {
        float damage = (impactVelocity - ItemInformation.safeFallVelocity) *damageMultiplier;
        ItemInformation.value -= damage;
        Debug.Log($"player took {damage} fall damage! Health is now {ItemInformation.value}");
       } 
       else
       {
        Debug.Log("player landed safely. No damge taken.");
       }
    }
}