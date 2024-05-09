using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{//Character
    BaseMovementState currentState;
    public CrouchingState CrouchingState;
    public SprintState SprintState;
    public WalkingState WalkingState;
    public StandingState StandingState;

    //Controller
    //Animator
    //playerInput
    //camera transform
    //player velocity
    //normalcollider height
    //gravity value
    //Animation smoothing stuff
    //Controls speeds?

    private void Start()
    {
        currentState = StandingState;
        currentState.Enter();

        CrouchingState = new CrouchingState(this);
        SprintState = new SprintState(this);
        WalkingState = new WalkingState(this);
        StandingState = new StandingState(this);
    }

    private void Update()
    {
        //currentState.UpdateState();
        currentState.HandleInput();
        currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        currentState.PhysicsUpdate();
    }

    public void SwitchState(BaseMovementState state)
    {
        currentState.Exit();
        currentState = state;
        state.Enter();
    }
}
