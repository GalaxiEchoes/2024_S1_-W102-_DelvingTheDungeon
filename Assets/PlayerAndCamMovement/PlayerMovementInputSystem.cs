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
    bool grounded;

    [Header("Stair Handling")]
    public GameObject stepRayUpper;
    public GameObject stepRayLower;
    public float maxStepHeight;
    public float stepSmooth;
    public LayerMask stairs;
    private bool onStairs;
    private RaycastHit stairHit;
    private float stairsDownTimer = 0f;
    private float stairsDownDelay = 0.2f;
    public float raycastDistanceScalingFactor;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    public float slopeSmooth;
    private RaycastHit slopeHit;
    private bool onSlope;

    //Other Variables
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
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
        onSlope = calcOnSlope();

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
        if ((InputManager.instance.JumpJustPressed || InputManager.instance.JumpBeingHeld) && readyToJump && (grounded || onStairs || onSlope))
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

    private void SpeedControl()//limit velocity....
    {
        //Limit speed on slope or stairs
        if (onStairs || onSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
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
        else if (InputManager.instance.SprintBeingHeld && (grounded || onStairs))
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

        setGravity();
        handleMovement();
    }

    private void setGravity()
    {
        //Handles gravity
        if ((calcOnSlope() || onStairs) && readyToJump)
        {
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }
    }

    private void handleMovement()
    {
        if (stepObstacleCheck(moveDirection) && moveDirection.magnitude > 0) //Is there an obstacle and are we moving
        {
            if (onSlope) //Any incline
            {
                if (onStairs) //On stairs with incline
                {
                    Debug.Log("Stairs with incline");
                    rb.transform.Translate(Vector3.up * slopeSmooth * Time.deltaTime * moveSpeed);
                    rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
                }
                else //On slight slope
                {
                    Debug.Log("Slight Slope");
                    rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
                    if (rb.velocity.y > 0)
                    {
                        rb.AddForce(Vector3.down * 80f, ForceMode.Force);
                    }
                }
            }
            else //Bump/stair handling
            {
                Debug.Log("Bump handling");
                rb.transform.Translate(Vector3.up * stepSmooth * Time.deltaTime);
                rb.AddForce(moveDirection.normalized * (moveSpeed * 1.75f) * 20f, ForceMode.Force);
            }

            stairsDownTimer = 0f;
        }
        else if (!stepObstacleCheck(moveDirection) && readyToJump) //Are there no obstacles
        {
            if (onStairs)//Down Stairs
            {
                Debug.Log("Down Stairs");
                stairsDownTimer += Time.deltaTime;

                if (stairsDownTimer >= stairsDownDelay)
                {
                    rb.AddForce(Vector3.down * 0.7f, ForceMode.Impulse);
                    rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
                }
            }
            else if (onSlope)//Down Slope
            {
                rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
                if (rb.velocity.y > 0)
                {
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
                }
            }
            else if (grounded) //On the ground
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }
            else //In the air
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }
        }
        else if (grounded || onStairs || onSlope) //Flat area handling
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else //In the Air
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

    bool stepObstacleCheck(Vector3 direction)
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, direction, out hitLower, 0.8f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, direction, out hitUpper, 1f))
            {
                return true;
            }
        }
        return false;
    }

    private bool calcOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}