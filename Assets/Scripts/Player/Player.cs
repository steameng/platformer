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
    public float dashVelocity = 10;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    PlayerState playerState;
    public Animator Animator;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2); // Calculate gravity
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex; // Calculate jump velocity
        jumps = jumpsMax; // Set max jumps
        playerState.Reset();
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);

    }

    void Update()
    {
        // if (Animator.GetCurrentAnimatorStateInfo(0).IsName("IdleAlucard"))
        // {
        //     Animator.SetBool("Backdash", false);
        // }
        // Hitting the ceiling
        if (controller.collisions.above)
        {
            velocity.y = 0;
        }

        // Hitting the floor
        if (controller.collisions.below)
        {
            velocity.y = 0; // Don't accumulate gravity
            jumps = jumpsMax;
            Animator.SetBool("Grounded", true);
            Animator.SetBool("IsFalling", false);
        }
        if (!controller.collisions.below)
        {
            Animator.SetBool("Grounded", false); 
        }

        // Direction and Speed
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
   
 
        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && jumps > 0)
        {
            velocity.y = jumpVelocity;
            jumps -= 1;
            // Animator.SetBool("Grounded", false);
            Animator.SetTrigger("Jump");
        }


        // Dashing
        if (Input.GetKeyDown(KeyCode.Q))
        {
            float backdashDirection = (facingRight) ? -1 : 1;
            velocity.x = dashVelocity * backdashDirection;
            Animator.SetTrigger("Backdash");
        }

        


        // Moving
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Mathf.Sign(velocity.y) == -1 && !Animator.GetBool("Grounded"))
        {
            Animator.SetBool("IsFalling", true);
        }

        if (!Animator.GetBool("Backdash"))
        {
            Animator.SetFloat("Speed", Mathf.Abs(velocity.x));
        }
        

        print(
        "XInput: " + input.x +
        "YInput: " + input.y +
        "XVelocity: " + velocity.x +
        "YVelocity: " + velocity.y
        );


        // Sprite Facing Direction
        if (facingRight && input.x < 0 && !Animator.GetBool("Backdash"))
        {
            Flip();
        }
        if (!facingRight && input.x > 0 && !Animator.GetBool("Backdash"))
        {
            Flip();
        }

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

    void Jump(float jumpVelocity)
    {
        velocity.y = jumpVelocity;
        jumps -= 1;
    }

     public struct PlayerState
     {
        public bool backdashing;
        
        public void Reset()
        {
            backdashing =  false;
        }
     
    }


}