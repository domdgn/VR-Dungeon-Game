using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class advancedMovement : MonoBehaviour
{

    public InputActionReference jumpButton = null;
    public CharacterController charController;
    public float jumpHeight;
    private float gravityValue = -9.81f;

    private Vector3 playerVelocity;

    public bool jumpButtonReleased;

    private bool isTouchingGround;
    // Start is called before the first frame update
    void Start()
    {
        jumpButtonReleased = true;
    }

    // Update is called once per frame
    void Update()
    {

        playerVelocity.y += gravityValue * Time.deltaTime;
        charController.Move(playerVelocity * Time.deltaTime);
        if (charController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            isTouchingGround = true;
        }

        float jumpVal = jumpButton.action.ReadValue<float>();
        if (jumpVal > 0 && jumpButtonReleased == true)
        {
            jumpButtonReleased = false;
            Jump();
            isTouchingGround = false;
        }
        else if (jumpVal == 0)
        {
            jumpButtonReleased = true;
        }
    }

    public void Jump()
    {
        if (isTouchingGround == false)
        {
            return;
        }
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }
}