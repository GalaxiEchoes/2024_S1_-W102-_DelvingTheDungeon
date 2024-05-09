using UnityEngine;

public abstract class BaseMovementState
{
    public MovementStateManager StateManager;
    
    public BaseMovementState(MovementStateManager stateManager)
    {
        this.StateManager = stateManager;
    }

    public abstract void Enter();
    public abstract void UpdateState();

    public abstract void HandleInput();

    public abstract void LogicUpdate();

    public abstract void PhysicsUpdate();

    public abstract void Exit();

}
