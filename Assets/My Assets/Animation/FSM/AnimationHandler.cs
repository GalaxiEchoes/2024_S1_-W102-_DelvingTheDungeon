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

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    void ChangeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, float max)
    {
        if (forwardPressed && zVelocity < max)
        {
            zVelocity += Time.deltaTime * acceleration;
        }

        if (leftPressed && xVelocity > -max)
        {
            xVelocity -= Time.deltaTime * acceleration;
        }

        if (rightPressed && xVelocity < max)
        {
            xVelocity += Time.deltaTime * acceleration;
        }

        if (!forwardPressed && zVelocity > 0.0f)
        {
            zVelocity -= Time.deltaTime * deceleration;
        }

        if (!leftPressed && xVelocity < 0.0f)
        {
            xVelocity += Time.deltaTime * deceleration;
        }

        if (!rightPressed && xVelocity > 0.0f)
        {
            xVelocity -= Time.deltaTime * deceleration;
        }
    }

    void LockResetVelocity(bool forwardPressed, bool leftPressed, bool rightPressed,bool runPressed, float max)
    {
        if (!forwardPressed && zVelocity < 0.0f)
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
            xVelocity -= Time.deltaTime * deceleration;
            if (xVelocity < -max && xVelocity > (-max - 0.05))
            {
                xVelocity = -max;
            }
        }
        else if (leftPressed && xVelocity > -max && xVelocity < (-max + 0.05f))
        {
            xVelocity = -max;
        }

        if (rightPressed && runPressed && xVelocity > max)
        {
            xVelocity = max;
        }
        else if (rightPressed && xVelocity > max)
        {
            xVelocity -= Time.deltaTime * deceleration;
            if (xVelocity > max && xVelocity < (max + 0.05))
            {
                xVelocity = max;
            }
        }
        else if (rightPressed && xVelocity < max && xVelocity > (max - 0.05f))
        {
            xVelocity = max;
        }
    }


    public void Update()
    {
        bool runPressed = InputManager.instance.SprintBeingHeld;
        bool forwardPressed = InputManager.instance.MoveInput.y > 0;
        bool leftPressed = InputManager.instance.MoveInput.x < 0;
        bool rightPressed = InputManager.instance.MoveInput.x > 0;

        float max = runPressed? maxRunVelocity : maxWalkVelocity;

        ChangeVelocity(forwardPressed, leftPressed, rightPressed, max);
        LockResetVelocity(forwardPressed, leftPressed, rightPressed, runPressed, max);

        animator.SetFloat("VelocityZ", zVelocity);
        animator.SetFloat("VelocityX", xVelocity);
    }












    /*
    Animator animator;
    float velocity= 0.0f;
    public float acceleration = 0.1f;
    public float deceleration = 0.1f;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left ctrl");

        if (forwardPressed && velocity < 1) {
            velocity += Time.deltaTime * acceleration;
        }

        if(!forwardPressed && velocity > 0.0f)
        {
            velocity -= Time.deltaTime * deceleration;
        }

        if(velocity < 0.0f) velocity = 0.0f;

        animator.SetFloat("Velocity", velocity);
    }
    */


    /*public void Update()
    {
        bool isRunning = animator.GetBool("IsRunning");
        bool isWalking = animator.GetBool("IsWalking");
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");

        if (!isWalking && forwardPressed)
        {
            animator.SetBool("IsWalking", true);
        }
        
        if (isWalking && !forwardPressed) 
        {
            animator.SetBool("IsWalking", false);
        }

        if (!isRunning && (forwardPressed && runPressed)) 
        {
            animator.SetBool("IsRunning", true);
        }

        if (isRunning &&(!runPressed || !forwardPressed)) 
        {
            animator.SetBool("IsRunning", false);
        }
    }*/
}
