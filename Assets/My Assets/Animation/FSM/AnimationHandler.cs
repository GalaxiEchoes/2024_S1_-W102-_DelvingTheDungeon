using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.TextCore.Text;

public class AnimationHandler : MonoBehaviour
{
    float xVelocity = 0.0f;
    float zVelocity = 0.0f;
    public float acceleration = 2f;
    public float deceleration = 2f;
    public float maxWalkVelocity = 0.5f;
    public float maxRunVelocity = 1f;

    Animator animator;
    InputSystemPlayerMovement PlayerMovement;
    public PlayerActions PlayerActions;
    public GameObject Player;
    private bool isDrawn;

    public void Start()
    {
        isDrawn = false;
        animator = Player.GetComponentInChildren<Animator>();
        PlayerMovement = Player.GetComponent<InputSystemPlayerMovement>();
    }

    public void Update()
    {
        bool runPressed = InputManager.instance.SprintBeingHeld && PlayerMovement.state == InputSystemPlayerMovement.MovementState.sprinting;
        bool forwardPressed = InputManager.instance.MoveInput.y > 0;
        bool backPressed = InputManager.instance.MoveInput.y < 0;
        bool leftPressed = InputManager.instance.MoveInput.x < 0;
        bool rightPressed = InputManager.instance.MoveInput.x > 0;
        bool crouchPressed = InputManager.instance.CrouchBeingHeld;
        bool jumpPressed = PlayerMovement.readyToJump == false;
        bool isGrounded = Physics.Raycast(Player.transform.position, Vector3.down, PlayerMovement.playerHeight * 0.5f + 0.2f, PlayerMovement.ground);
        bool isFalling = PlayerMovement.state == InputSystemPlayerMovement.MovementState.air;
        bool drawOrSheathPressed = InputManager.instance.DrawOrSheathWeapon;
        bool attackPressed = InputManager.instance.SimpleAttack;
        //bool strongAttackPressed = InputManager.instance.StrongAttack;

        float max = runPressed && !crouchPressed ? maxRunVelocity : maxWalkVelocity;

        ChangeVelocity(forwardPressed, leftPressed, rightPressed, backPressed, max);
        LockResetVelocity(forwardPressed, leftPressed, rightPressed, runPressed, backPressed, max);

        if (zVelocity > 0.05 || zVelocity < -0.05 || xVelocity > 0.05 || xVelocity < -0.05) animator.SetBool("IsMoving", true);
        else animator.SetBool("IsMoving", false);

        //if (attackPressed && PlayerActions.isAttacking) animator.SetTrigger("SimpleAttack");
        //if (strongAttackPressed && PlayerActions.isAttacking) animator.SetTrigger("StrongAttack");
        if (!jumpPressed) animator.SetBool("IsGrounded", isGrounded);
        if (drawOrSheathPressed)
        {
            isDrawn = !isDrawn;
            if (isDrawn) animator.SetTrigger("DrawWeapon");
            else animator.SetTrigger("SheathWeapon");

        }
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsJumping", jumpPressed); //Need to make sure if crouched no jumping?
        animator.SetBool("IsCrouched", crouchPressed);

        HandleAttack(attackPressed);

        //If attacking, no moving just root motion
        if (!isAttacking)
        {
            animator.SetFloat("VelocityZ", zVelocity);
            animator.SetFloat("VelocityX", xVelocity);
        }
        else
        {
            animator.SetFloat("VelocityZ", 0.0f);
            animator.SetFloat("VelocityX", 0.0f);
        }
    }

    void ChangeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool backPressed, float max)
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

    void LockResetVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, bool backPressed, float max)
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


    float timePassed = 0f;
    float clipLength;
    float clipSpeed;
    bool isAttacking = false;

    void HandleAttack(bool attackPressed)
    {
        if (attackPressed) //Attack has just been pressed
        {
            isAttacking = true;
            animator.SetTrigger("SimpleAttack");
            animator.applyRootMotion = true;
        }
        
        if(isAttacking)
        {

        }








        /*if (attackPressed && !isAttacking)//Enter Attack state
        {
            animator.applyRootMotion = true;
            timePassed = 0f;
            animator.SetTrigger("SimpleAttack");
        }

        if (attackPressed)
        {
            isAttacking = true;
        }

        if (isAttacking)
        {
            timePassed += Time.deltaTime;
            clipLength = animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
            clipSpeed = animator.GetCurrentAnimatorStateInfo(1).speed;

            if(timePassed >= clipLength / clipSpeed && isAttacking)
            {
                //Still attacking, restart
                isAttacking = false;
                HandleAttack(attackPressed);
            }
            if (timePassed >= clipLength / clipSpeed)
            {
                animator.SetTrigger("StopAttacking"); //attacking state expired?
            }
        }*/
    }

    /*
 float timePassed;
float clipLength;
float clipSpeed;
bool isAttacking; on entering state is false
//enter
apply root motion character.animator.applyRootMotion = true
timePassed = 0f;
animator.setTrigger("attack");
animator.setFloat("Speed", 0 ); //either set velocity or not needed?


handling input, if left mouse clicked, attack = true


LogicUpdate

timePassed += Time.deltaTime;
clipLength = animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
clipSpeed = animator.GetCurrentAnimatorStateInfo(1).speed;

if(timePassed>= cliplength / clipSpeed && attack){
    statemachine.changestate(character.attacking); ->attack state enter?
}
if(timepassed >=cliplength/clipspeed){
    statemachine.changestate(character.combatting); -> normal combat state
    character.animator.SetTrigger("move")
}

exit -> rootmotion = false
 */
}
