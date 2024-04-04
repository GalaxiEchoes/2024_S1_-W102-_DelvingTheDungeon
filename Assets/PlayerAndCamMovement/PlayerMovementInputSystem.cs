using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystemPlayerMovement : MonoBehaviour
{
    public enum MovementState
    {
        walking,
        sprinting,
        air,
        crouching
    }

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float groundDrag;
    public MovementState state;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    private bool grounded;

    [Header("Stair/Slope Handling")]
    public float maxSlopeAngle;
    public LayerMask stairs;
    private bool onStairs;
    private RaycastHit stairHit;

    public GameObject stepRayUpper;
    public GameObject stepRayLower;
    public float maxStepHeight;
    public float stepSmooth;
    public float slopeSmooth;

    //Other Variables
    [Header("Movement Orientation")]
    public Transform orientation;
    private float horizontalInput;
    private float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
    }
    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
        onStairs = Physics.Raycast(transform.position, Vector3.down, out stairHit, playerHeight * 0.5f + 0.3f, stairs);

        MyInput();
        SpeedControl();
        StateHandler();

        if (grounded || onStairs)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void MyInput()
    {
        horizontalInput = InputManager.instance.MoveInput.x;
        verticalInput = InputManager.instance.MoveInput.y;

        //When to jump
        if (InputManager.instance.JumpJustPressed && readyToJump && (grounded || onStairs))
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //Crouching
        if (InputManager.instance.CrouchJustPressed)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        //Stop crouching
        if (InputManager.instance.CrouchReleased)
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void SpeedControl()
    {
        if (onStairs)
        {
            if (rb.velocity.magnitude > moveSpeed * 1.75f)
            {
                rb.velocity = rb.velocity.normalized * (moveSpeed * 1.75f);
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            //Limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

    }

    private void StateHandler()
    {
        if (InputManager.instance.CrouchBeingHeld)
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        if ((grounded || onStairs) && InputManager.instance.SprintBeingHeld)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded || onStairs)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        //Calc movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (stepClimb() && moveDirection.magnitude > 0)
        {
            if (OnSlope())
            {
                rb.transform.Translate(Vector3.up * slopeSmooth * Time.deltaTime);
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }
            else
            {
                rb.transform.Translate(Vector3.up * stepSmooth * Time.deltaTime);
                rb.AddForce(moveDirection.normalized * (moveSpeed * 1.75f) * 20f, ForceMode.Force);
            }


        }
        else if (onStairs && !stepClimb() && readyToJump)
        {
            rb.AddForce(Vector3.down * 0.5f, ForceMode.Impulse);
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (grounded)//On ground
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded) //In air
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void Jump()
    {
        //Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private bool stepClimb()
    {
        bool climbing = false;

        climbing = rayCastCheck(orientation.transform.TransformDirection(Vector3.forward));
        climbing = climbing || rayCastCheck(orientation.transform.TransformDirection(1.5f, 0, 1));
        climbing = climbing || rayCastCheck(orientation.transform.TransformDirection(-1.5f, 0, 1));

        return climbing;
    }

    bool rayCastCheck(Vector3 direction)
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, direction, out hitLower, 0.6f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, direction, out hitUpper, 1f))
            {
                return true;
            }
        }
        return false;
    }

    private bool OnSlope()
    {
        RaycastHit slopeHit;
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
}

