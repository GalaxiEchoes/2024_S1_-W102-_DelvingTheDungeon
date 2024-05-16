using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    //User Input References
    public static InputManager instance;
    public static PlayerInput PlayerInput;

    //Movement
    public Vector2 MoveInput { get; private set; }
    public bool JumpJustPressed { get; private set; }
    public bool JumpBeingHeld { get; private set; }
    public bool CrouchJustPressed { get; private set; }
    public bool CrouchReleased { get; private set; }
    public bool CrouchBeingHeld { get; private set; }
    public bool SprintBeingHeld { get; private set; }
    public bool FirstPersonCamPressed { get; private set; }
    public bool CombatCamPressed { get; private set; }
    public bool ThirdPersonCamPressed { get; private set; }
    public bool SimpleAttack { get; private set; }
    public bool DrawOrSheathWeapon { get; private set; }

    //Actual Keys
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _crouchAction;
    private InputAction _sprintAction;
    private InputAction _InteractAction;
    private InputAction _firstPersonCamAction;
    private InputAction _combatCamAction;
    private InputAction _thirdPersonCamAction;
    private InputAction _attackAction;
    private InputAction _drawOrSheathAction;

    //UI interaction
    public bool MenuOpenCloseInput { get; private set; }
    public bool InventoryOpen { get; private set; }
    public bool InventoryClose { get; private set; }
    public bool Interact { get; private set; }
    public bool UIMenuCloseInput { get; private set; }

    //Actual Keys
    private InputAction _menuOpenCloseAction;
    private InputAction _InventoryOpenAction;
    private InputAction _UIInventoryCloseAction;
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
        _crouchAction = PlayerInput.actions["Crouch"];
        _sprintAction = PlayerInput.actions["Sprint"];
        _InteractAction = PlayerInput.actions["Interact"];
        _firstPersonCamAction = PlayerInput.actions["First Person Cam"];
        _combatCamAction = PlayerInput.actions["Combat Cam"];
        _thirdPersonCamAction = PlayerInput.actions["Third Person Cam"];
        _menuOpenCloseAction = PlayerInput.actions["Menu"];
        _InventoryOpenAction = PlayerInput.actions["Inventory"];
        _UIMenuCloseAction = PlayerInput.actions["MenuCloseAction"];
        _UIInventoryCloseAction = PlayerInput.actions["InventoryClose"];
        _attackAction = PlayerInput.actions["Attack"];
        _drawOrSheathAction = PlayerInput.actions["DrawOrSheath"];
    }

    private void UpdateInputs()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        JumpJustPressed = _jumpAction.WasPressedThisFrame();
        JumpBeingHeld = _jumpAction.IsPressed();
        CrouchJustPressed = _crouchAction.WasPressedThisFrame();
        CrouchReleased = _crouchAction.WasReleasedThisFrame();
        CrouchBeingHeld = _crouchAction.IsPressed();
        SprintBeingHeld = _sprintAction.IsPressed();
        Interact = _InteractAction.WasPressedThisFrame();
        FirstPersonCamPressed = _firstPersonCamAction.WasPressedThisFrame();
        CombatCamPressed = _combatCamAction.WasPressedThisFrame();
        ThirdPersonCamPressed = _thirdPersonCamAction.WasPressedThisFrame();
        MenuOpenCloseInput = _menuOpenCloseAction.WasPressedThisFrame();
        UIMenuCloseInput = _UIMenuCloseAction.WasPressedThisFrame();
        InventoryOpen = _InventoryOpenAction.WasPressedThisFrame();
        InventoryClose = _UIInventoryCloseAction.WasPressedThisFrame();
        SimpleAttack = _attackAction.WasPressedThisFrame();
        DrawOrSheathWeapon = _drawOrSheathAction.WasPressedThisFrame();
    }

    void Update()
    {
        UpdateInputs();
    }
}
