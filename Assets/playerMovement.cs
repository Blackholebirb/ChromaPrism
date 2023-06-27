using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    float maxSpeed = 0.5f;
    float accelaration = 1.02f;
    float xVel = 0;
    float yVel = 0;
    float gravity = 0.01f;
    float jumpStrength = 0.3f;
    float tapJump = 0.2f;
    float holdJump =0.008f;
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
        float xTrans = Input.GetAxis("Horizontal");
        float yTrans = Input.GetAxis("Jump");
        float xDiff = xTrans - xVel;

        xVel += 0.5f * xDiff;
        if (yTrans > 0 && isGrounded)
        {
            yVel += tapJump;
            isGrounded = false;
        }
        
        if (yTrans > 0 && yVel > 0)
        {
            yVel += holdJump;
        }
        
        if (!isGrounded)
        {
            yVel -= gravity;
        }

        transform.Translate((0.3f*xVel), yVel, 0);
    }
}
