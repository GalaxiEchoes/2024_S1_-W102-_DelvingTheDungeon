using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("User Input")]
    public static InputManager instance;

    public static PlayerInput PlayerInput;

    //UI interaction
    public bool MenuOpenCloseInput { get; private set; }
    public bool InventoryOpenClose { get; private set; }
    public bool Interact { get; private set; }

    public bool FirstPersonCamPressed { get; private set; }
    public bool CombatCamPressed { get; private set; }
    public bool ThirdPersonCamPressed { get; private set; }
    public bool UIMenuCloseInput { get; private set; }

    //Actual Keys
    private InputAction _menuOpenCloseAction;
    //private InputAction _InventoryOpenCloseAction;
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
        _menuOpenCloseAction = PlayerInput.actions["Menu"];
        //_InventoryOpenCloseAction = PlayerInput.actions["Inventory"];
        _UIMenuCloseAction = PlayerInput.actions["MenuCloseAction"];
    }

    private void UpdateInputs()
    {
        MenuOpenCloseInput = _menuOpenCloseAction.WasPressedThisFrame();
        UIMenuCloseInput = _UIMenuCloseAction.WasPressedThisFrame();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputs();
    }
}
