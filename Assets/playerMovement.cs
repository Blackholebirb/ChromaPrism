using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    float xVel = 0, yVel = 0;
    float maxXSpeed = 0.3f;
    float xAccelFactor = 0.5f;
    float gravity = 0.01f;
    float tapJumpHeight = 5;
    float holdJumpHeight = 10;
    bool isGrounded = true;

    // In order to get collision to work properly, the two colliding objects need to have Rigidbody 2D,
    // must be simulated, gravity scale 0, and one of them needs to have isTrigger = true
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isGrounded = true;
        transform.Translate(0, -yVel, 0);
        yVel = 0f;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Input variables; note that keyboard inputs do not drop immediately to zero after being released
        // They instead gradually ramp down over a second or so
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Jump");
        float keyboardBuffer = 0.2f;

        // Horizontal movement - acceleration towards the input value, with higher acceleration occurring
        // when farther away from the target velocity.
        float xDiff = xInput - xVel;
        xVel += xAccelFactor * xDiff;

        // Vertical movement - typical gravity, with a jump that can be made taller or shorter based on how
        // long the jump button is held
        if (yInput > keyboardBuffer && isGrounded)
        {
            // Initial push when jumping off the ground
            yVel += Mathf.Sqrt(2 * gravity * tapJumpHeight);
            isGrounded = false;
        }
        if (yInput > 1 - keyboardBuffer && yVel > 0)
        {
            // Higher jump from holding the input
            yVel += gravity * (1f - tapJumpHeight / holdJumpHeight);
        }
        if (!isGrounded)
        {
            // Gravity, only applying when in the air
            yVel -= gravity;
        }

        transform.Translate(maxXSpeed * xVel, yVel, 0);
    }
}
