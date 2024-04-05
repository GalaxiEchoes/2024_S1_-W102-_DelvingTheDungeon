using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("User Input")]
    public static InputManager instance;

    public static PlayerInput PlayerInput;

    //Movement
    public Vector2 MoveInput { get; private set; }
    public bool JumpJustPressed { get; private set; }
    public bool JumpBeingHeld { get; private set; }
    public bool AttackInput { get; private set; }
    public bool CrouchJustPressed { get; private set; }
    public bool CrouchReleased { get; private set; }
    public bool CrouchBeingHeld { get; private set; }
    public bool SprintBeingHeld { get; private set; }

    //UI interaction
    public bool MenuOpenCloseInput { get; private set; }
    public bool InventoryOpenClose { get; private set; }
    public bool Interact { get; private set; }

    public bool FirstPersonCamPressed { get; private set; }
    public bool CombatCamPressed { get; private set; }
    public bool ThirdPersonCamPressed { get; private set; }
    public bool UIMenuCloseInput { get; private set; }

    //Actual Keys
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private InputAction _crouchAction;
    private InputAction _sprintAction;
    private InputAction _menuOpenCloseAction;
    private InputAction _InventoryOpenCloseAction;
    private InputAction _InteractAction;
    private InputAction _firstPersonCamAction;
    private InputAction _combatCamAction;
    private InputAction _thirdPersonCamAction;
    private InputAction _UIMenuCloseAction;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        PlayerInput = GetComponent<PlayerInput>();

        SetupInputActions();
    }

    private void SetupInputActions()
    {
        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _attackAction = PlayerInput.actions["Attack"];
        _crouchAction = PlayerInput.actions["Crouch"];
        _sprintAction = PlayerInput.actions["Sprint"];
        _menuOpenCloseAction = PlayerInput.actions["Menu"];
        _InventoryOpenCloseAction = PlayerInput.actions["Inventory"];
        _InteractAction = PlayerInput.actions["Interact"];
        _firstPersonCamAction = PlayerInput.actions["First Person Cam"];
        _combatCamAction = PlayerInput.actions["Combat Cam"];
        _thirdPersonCamAction = PlayerInput.actions["Third Person Cam"];
        _UIMenuCloseAction = PlayerInput.actions["MenuCloseAction"];
    }

    private void UpdateInputs()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        JumpJustPressed = _jumpAction.WasPressedThisFrame();
        JumpBeingHeld = _jumpAction.IsPressed();
        AttackInput = _attackAction.WasPressedThisFrame();
        CrouchJustPressed = _crouchAction.WasPressedThisFrame();
        CrouchReleased = _crouchAction.WasReleasedThisFrame();
        CrouchBeingHeld = _crouchAction.IsPressed();
        SprintBeingHeld = _sprintAction.IsPressed();
        MenuOpenCloseInput = _menuOpenCloseAction.WasPressedThisFrame();
        InventoryOpenClose = _InventoryOpenCloseAction.WasPressedThisFrame();
        Interact = _InteractAction.WasPressedThisFrame();
        FirstPersonCamPressed = _firstPersonCamAction.WasPressedThisFrame();
        CombatCamPressed = _combatCamAction.WasPressedThisFrame();
        ThirdPersonCamPressed = _thirdPersonCamAction.WasPressedThisFrame();
        UIMenuCloseInput = _UIMenuCloseAction.WasPressedThisFrame();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputs();
    }
}
