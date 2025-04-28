using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDamagehandler : MonoBehaviour
{
    private Rigidbody rb; // Rigidbody reference to get velocity
    private bool isGrounded = false; // Tracks whether the object is touching the ground
    private bool onWall = false; // tracks whether the object is touching the wall 
    private float lastYVelocity = 0f; // Stores vertical velocity while falling
    private float lastXVelocity = 0f; // Stores horizontal velocity while falling 
    private IDamageable damageable; // Interface reference to apply damage

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get Rigidbody component
        damageable = GetComponent<IDamageable>(); // Get the damageable component (must implement IDamageable)
    }

    void Update()
    {
        // While the object is in the air, track the downward velocity
        if (!isGrounded)
        {
            lastYVelocity = rb.velocity.y;
            lastXVelocity =  rb.velocity.x;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object lands on the ground and wasn't already grounded
        if (!isGrounded && collision.gameObject.CompareTag("Ground"))
        {
            // Calculate impact speed from the last recorded fall speed
            float impactVelocity = Mathf.Abs(lastYVelocity);

            // If the object has a fall damage handler, apply the damage
            damageable?.Damage(impactVelocity);

            // Mark object as grounded again
            isGrounded = true;
        }
        
        if (!onWall && !isGrounded && collision.gameObject.CompareTag("Wall"))
        {
            float impactVelocity = Mathf.Abs(lastXVelocity);
            damageable?.Damage(impactVelocity);
            onWall = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // When the object leaves the ground, it's considered falling again
        if (collision.gameObject.CompareTag("Ground") && collision.gameObject.CompareTag("Wall"))
        {
            isGrounded = false;
        }
    } 
}
