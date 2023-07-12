using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    private enum CollisionDirection
    {
        Up      = 0,
        Down    = 1,
        Left    = 2,
        Right   = 3
    }

    float xVel = 0, yVel = 0;
    float maxXSpeed = 0.3f;
    float xAccelFactor = 0.5f;
    float gravity = 0.01f;
    float tapJumpHeight = 5;
    float holdJumpHeight = 10;
    bool isGrounded = true;

    // TODO: This could probably use some further refinement because it's still a bit buggy
    private CollisionDirection CheckCollisionDirection(Collider2D worldObj)
    {
        Vector2 contactPoint = worldObj.ClosestPoint(transform.position);
        Vector2 center = worldObj.bounds.center;
        float xMin = center.x - worldObj.bounds.size.x / 2;
        float xMax = center.x + worldObj.bounds.size.x / 2;
        float yMin = center.y - worldObj.bounds.size.y / 2;
        float yMax = center.y + worldObj.bounds.size.y / 2;

        float collsionThickness = 0.05f;
        if (Mathf.Abs(yMax - contactPoint.y) < collsionThickness)
            return CollisionDirection.Down;
        else if (Mathf.Abs(yMin - contactPoint.y) < collsionThickness)
            return CollisionDirection.Up;
        else if (Mathf.Abs(xMin - contactPoint.x) < collsionThickness)
            return CollisionDirection.Right;
        else
            return CollisionDirection.Left;
    }

    // In order to get collision to work properly, the two colliding objects need to have Rigidbody 2D,
    // must be simulated, gravity scale 0, and one of them needs to have isTrigger = true
    private void OnTriggerEnter2D(Collider2D worldObj)
    {
        CollisionDirection dir = CheckCollisionDirection(worldObj);
        switch (dir)
        {
            case CollisionDirection.Down:
                // Bottom: Cancel all vertical movement so gravity doesn't pull through the ground
                isGrounded = true;
                transform.Translate(0, -yVel, 0);
                yVel = 0;
                break;
            case CollisionDirection.Up:
                // Top: Bounce back with 20% speed
                transform.Translate(0, -yVel, 0);
                yVel = -0.2f * yVel;
                break;
            case CollisionDirection.Left:
            case CollisionDirection.Right:
                // Sides: Cancel all horizontal movement
                transform.Translate(-xVel, 0, 0);
                xVel = 0;
                break;
        }
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
