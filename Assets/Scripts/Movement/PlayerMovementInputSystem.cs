using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSystemPlayerMovement : MonoBehaviour
{
    public enum MovementState
    {
        walking,
        sprinting,
        air,
        crouching
    }

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float groundDrag;
    private float moveSpeed;
    public MovementState state;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private bool readyToJump;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask ground;
    private bool grounded;

    [Header("Stair Handling")]
    [SerializeField] private GameObject stepRayUpper;
    [SerializeField] private GameObject stepRayLower;
    [SerializeField] private float maxStepHeight;
    [SerializeField] private float stepSmooth;
    [SerializeField] private LayerMask stairs;

    private bool onStairs;
    private RaycastHit stairHit;
    private float stairsDownTimer = 0f;
    private float stairsDownDelay = 0.2f;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private float slopeSmooth;
    private RaycastHit slopeHit;
    private bool onSlope;

    [Header("Object Reference")]
    [SerializeField] private Transform orientation;

    public Image StaminaBar;

    public float Stamina;
    public float MaxStamina;
    public float RunCost;
    public float ChargeRate;

    public Coroutine recharge;

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
        onSlope = CalcOnSlope();

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

    private void SpeedControl()
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
        else if (InputManager.instance.SprintBeingHeld && (grounded || onStairs) && Stamina > 0)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;

            Stamina -= RunCost * Time.deltaTime;
            if(Stamina < 0) Stamina = 0;
            StaminaBar.fillAmount = Stamina / MaxStamina;

            if(recharge != null)
            {
                StopCoroutine(recharge);
            }

            recharge = StartCoroutine(RechargeStamina());
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
        HandleMovement();
    }

    private void setGravity()
    {
        //Handles gravity
        if ((CalcOnSlope() || onStairs) && readyToJump)
        {
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }
    }

    private void HandleMovement()
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

    private bool CalcOnSlope()
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

    private IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f);

        while(Stamina < MaxStamina)
        {
            Stamina += ChargeRate / 10f;

            if (Stamina > MaxStamina)
            {
                Stamina = MaxStamina;
            }

            StaminaBar.fillAmount = Stamina / MaxStamina;
            yield return new WaitForSeconds(0.1f);
        }
    }
}