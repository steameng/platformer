using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    public float jumpHeight = 1.5f;
    public float timeToJumpApex = .3f;
    public int jumpsMax = 3;
    public int jumps;
    float accelerationTimeAirborne = .05f;
    float accelerationTimeGrounded = .05f;
    float moveSpeed = 3;
    private bool facingRight = true;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2); // Calculate gravity
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex; // Calculate jump velocity
        jumps = jumpsMax; // Set max jumps
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
    }

    void Update()
    {

        if (controller.collisions.above)
        {
            velocity.y = 0;
        }

        if (controller.collisions.below)
        {
            velocity.y = 0; // Don't accumulate gravity
            jumps = jumpsMax;
        }

        // Controller input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
               
        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && jumps > 0)
        {
            velocity.y = jumpVelocity;
            jumps -= 1;
        }

        // Moving
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Sprite Facing Direction
        if (facingRight && velocity.x < 0)
        {
            Flip();
        }
        if (!facingRight && velocity.x > 0)
        {
            Flip();
        }
    }

    void Jump(float jumpVelocity)
    {
        velocity.y = jumpVelocity;
        jumps -= 1;
    }

    void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}