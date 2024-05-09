using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationHandler : MonoBehaviour
{
    Animator animator;

    float xVelocity = 0.0f;
    float zVelocity = 0.0f;
    public float acceleration = 2f;
    public float deceleration = 2f;
    public float maxWalkVelocity = 0.5f;
    public float maxRunVelocity = 1f;
    public InputSystemPlayerMovement PlayerMovement;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    void ChangeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool backPressed, float max)
    {
        //Acceleration
        if (forwardPressed && zVelocity < max) zVelocity += Time.deltaTime * acceleration;
        if(backPressed && zVelocity > -max) zVelocity -= Time.deltaTime * acceleration;
        if (leftPressed && xVelocity > -max) xVelocity -= Time.deltaTime * acceleration;
        if (rightPressed && xVelocity < max) xVelocity += Time.deltaTime * acceleration;

        //Deceleration
        if (!forwardPressed && zVelocity > 0.0f) zVelocity -= Time.deltaTime * deceleration;
        if(!backPressed && zVelocity < 0.0f) zVelocity += Time.deltaTime * deceleration;
        if (!leftPressed && xVelocity < 0.0f) xVelocity += Time.deltaTime * deceleration;
        if (!rightPressed && xVelocity > 0.0f) xVelocity -= Time.deltaTime * deceleration;
    }

    void LockResetVelocity(bool forwardPressed, bool leftPressed, bool rightPressed,bool runPressed,bool backPressed, float max)
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
            else if(zVelocity > -max && zVelocity < (-max + 0.05f)) zVelocity = -max;
            else if(zVelocity < -max)
            {
                zVelocity += Time.deltaTime * deceleration;
                if(zVelocity < -max && zVelocity > (-max -0.05f)) zVelocity = -max;
            }
        }
    }

    public void Update()
    {
        bool runPressed = InputManager.instance.SprintBeingHeld;
        //Check if we are out of stamina tho?

        bool forwardPressed = InputManager.instance.MoveInput.y > 0;
        bool backPressed = InputManager.instance.MoveInput.y < 0;
        bool leftPressed = InputManager.instance.MoveInput.x < 0;
        bool rightPressed = InputManager.instance.MoveInput.x > 0;
        bool crouchPressed = InputManager.instance.CrouchBeingHeld;
        bool jumpPressed = PlayerMovement.readyToJump == false;

        float max = runPressed && !crouchPressed? maxRunVelocity : maxWalkVelocity;

        ChangeVelocity(forwardPressed, leftPressed, rightPressed, backPressed, max);
        LockResetVelocity(forwardPressed, leftPressed, rightPressed, runPressed, backPressed, max);

        
        if(jumpPressed) animator.SetTrigger("IsJumping");
        animator.SetBool("IsCrouched", crouchPressed);
        animator.SetFloat("VelocityZ", zVelocity);
        animator.SetFloat("VelocityX", xVelocity);
    }
}
