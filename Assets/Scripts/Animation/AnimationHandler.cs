using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [Header("Velocity Variables")]
    float xVelocity = 0.0f;
    float zVelocity = 0.0f;
    public float acceleration = 2f;
    public float deceleration = 2f;
    public float maxWalkVelocity = 0.5f;
    public float maxRunVelocity = 1f;

    [Header("Player Reference")]
    public GameObject Player;
    public LayerMask groundTypes;

    //Local References
    private Animator animator;
    private InputSystemPlayerMovement PlayerMovement;
    private bool isDrawn;
    private bool isAttacking = false;
    public float drawCooldown = 1.5f;
    private float drawCooldownTimer = 0;
    private float prevHeight;
    private bool readyToJump;

    public void Start()
    {
        isDrawn = false;
        readyToJump = true;
        animator = Player.GetComponentInChildren<Animator>();
        PlayerMovement = Player.GetComponent<InputSystemPlayerMovement>();
        prevHeight = Player.transform.position.y;
    }
    
    public void Update()
    {
        //Limits Movement, Jumping or Crouching if in attack mode
        if (isAttacking && InputManager.instance.CrouchBeingHeld)
        {
            PlayerMovement.blockMoving = true;
            PlayerMovement.blockJump = true;
        }
        else if (isAttacking)
        {
            PlayerMovement.blockMoving = true;
            PlayerMovement.blockJump = true;
            PlayerMovement.blockCrouch = true;
        }
        else
        {
            PlayerMovement.blockMoving = false;
            PlayerMovement.blockJump = false;
            PlayerMovement.blockCrouch = false;
        }

        HandleJumping();
        HandleAttack();
        HandleMovement();
    }

    private void HandleJumping()
    {
        //IsGrounded
        bool isGrounded = Physics.Raycast(Player.transform.position, Vector3.down, PlayerMovement.playerHeight * 0.5f + 0.7f, groundTypes) || PlayerMovement.onSlope || PlayerMovement.onStairs || PlayerMovement.grounded;
        animator.SetBool("IsGrounded", isGrounded);

        //IsFalling
        float currentHeight = Player.transform.position.y;
        if (Mathf.Abs(currentHeight - prevHeight) > 0.05f)
        {
            animator.SetBool("IsFalling", true);
        }
        else
        {
            animator.SetBool("IsFalling", false);
        }

        //IsJumping
        if ((InputManager.instance.JumpJustPressed || InputManager.instance.JumpBeingHeld) && readyToJump && (PlayerMovement.grounded || PlayerMovement.onStairs || PlayerMovement.onSlope) &&!isAttacking)
        {
            readyToJump = false;
            animator.SetTrigger("StartJump");
        }

        animator.SetBool("IsJumping", !readyToJump); 
    }

    private void HandleAttack()
    {
        //Button Bools
        bool drawOrSheathPressed = InputManager.instance.DrawOrSheathWeapon;
        bool attackPressed = InputManager.instance.SimpleAttack;

        if(drawCooldownTimer > 0)
        {
            drawCooldownTimer -= Time.deltaTime;
        }

        //Handles Drawing or Sheathing Weapon
        if (drawOrSheathPressed && drawCooldownTimer <= 0)
        {
            drawCooldownTimer = drawCooldown;
            isDrawn = !isDrawn;
            if (isDrawn) animator.SetTrigger("DrawWeapon");
            else animator.SetTrigger("SheathWeapon");
        }

        //Attacking
        if (attackPressed && isDrawn)
        {
            isAttacking = true;
            animator.SetTrigger("SimpleAttack");
        }
    }

    private void HandleMovement()
    {
        //WASD, Run and Crouch Bools
        bool runPressed = InputManager.instance.SprintBeingHeld && PlayerMovement.state == InputSystemPlayerMovement.MovementState.sprinting;
        bool forwardPressed = InputManager.instance.MoveInput.y > 0;
        bool backPressed = InputManager.instance.MoveInput.y < 0;
        bool leftPressed = InputManager.instance.MoveInput.x < 0;
        bool rightPressed = InputManager.instance.MoveInput.x > 0;
        bool crouchPressed = InputManager.instance.CrouchBeingHeld;

        //Setting the current Max Velocity
        float max = runPressed && !crouchPressed ? maxRunVelocity : maxWalkVelocity;

        //Changing Velocity based on inputs
        ChangeVelocity(forwardPressed, leftPressed, rightPressed, backPressed, max);
        LockResetVelocity(forwardPressed, leftPressed, rightPressed, runPressed, backPressed, max);

        //Setting Animator Parameters
        animator.SetBool("IsCrouched", crouchPressed);

        //Handling Velocity if attacking
        if (isAttacking)
        {
            animator.SetFloat("VelocityZ", 0.0f);
            animator.SetFloat("VelocityX", 0.0f);
            //Add call to InputManager to stop player from moving
        }
        else
        {
            animator.SetFloat("VelocityZ", zVelocity);
            animator.SetFloat("VelocityX", xVelocity);
        }

        //Sets if Bool for if player is moving
        if (zVelocity > 0.05 || zVelocity < -0.05 || xVelocity > 0.05 || xVelocity < -0.05) animator.SetBool("IsMoving", true);
        else animator.SetBool("IsMoving", false);
    }

    private void ChangeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool backPressed, float max)
    {
        //Acceleration
        if (forwardPressed && zVelocity < max) zVelocity += Time.deltaTime * acceleration;
        if (backPressed && zVelocity > -max) zVelocity -= Time.deltaTime * acceleration;
        if (leftPressed && xVelocity > -max) xVelocity -= Time.deltaTime * acceleration;
        if (rightPressed && xVelocity < max) xVelocity += Time.deltaTime * acceleration;

        //Deceleration
        if (!forwardPressed && zVelocity > 0.0f) zVelocity -= Time.deltaTime * deceleration;
        if (!backPressed && zVelocity < 0.0f) zVelocity += Time.deltaTime * deceleration;
        if (!leftPressed && xVelocity < 0.0f) xVelocity += Time.deltaTime * deceleration;
        if (!rightPressed && xVelocity > 0.0f) xVelocity -= Time.deltaTime * deceleration;
    }

    private void LockResetVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, bool backPressed, float max)
    {
        //Centeres to 0,0 to idle 
        if (!forwardPressed && !backPressed && (zVelocity < 0.05f && zVelocity > -0.05f))
        {
            zVelocity = 0.0f;
        }
        if (!rightPressed && !leftPressed && (xVelocity > -0.05f && xVelocity < 0.05f))
        {
            xVelocity = 0.0f;
        }

        if (forwardPressed && runPressed && zVelocity > max)
        {
            zVelocity = max;
        }
        else if (forwardPressed && zVelocity > max)
        {
            zVelocity -= Time.deltaTime * deceleration;
            if (zVelocity > max && zVelocity < (max + 0.05))
            {
                zVelocity = max;
            }
        }
        else if (forwardPressed && zVelocity < max && zVelocity > (max - 0.05f))
        {
            zVelocity = max;
        }

        if (leftPressed && runPressed && xVelocity < -max)
        {
            xVelocity = -max;
        }
        else if (leftPressed && xVelocity < -max)
        {
            xVelocity += Time.deltaTime * deceleration;
            if (xVelocity < -max && xVelocity > (-max - 0.05f))
            {
                xVelocity = -max;
            }
        }
        else if (leftPressed && xVelocity > -max && xVelocity < (-max + 0.05f))
        {
            xVelocity = -max;
        }

        //Handles right velocity cases
        if (rightPressed)
        {
            if (runPressed && xVelocity > max) xVelocity = max;
            else if (xVelocity < max && xVelocity > (max - 0.05f)) xVelocity = max;
            else if (xVelocity > max)
            {
                xVelocity -= Time.deltaTime * deceleration;
                if (xVelocity > max && xVelocity < (max + 0.05)) xVelocity = max;
            }
        }

        //Handles back velocity cases
        if (backPressed)
        {
            if (runPressed && zVelocity < -max) zVelocity = -max;
            else if (zVelocity > -max && zVelocity < (-max + 0.05f)) zVelocity = -max;
            else if (zVelocity < -max)
            {
                zVelocity += Time.deltaTime * deceleration;
                if (zVelocity < -max && zVelocity > (-max - 0.05f)) zVelocity = -max;
            }
        }
    }

    private void HandleAttackExit()
    {
        //Triggered through Animation Event
        if (!animator.GetBool("SimpleAttack"))
        {
            isAttacking = false;
            animator.SetTrigger("StopAttacking");
        }
    }

    private void ResetJump()
    {
        //Triggered through Animation Event
        readyToJump = true;
        PlayerMovement.ResetJump();
    }
}
