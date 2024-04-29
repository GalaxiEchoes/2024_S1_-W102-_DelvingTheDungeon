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
        _menuOpenCloseAction = PlayerInput.actions["Menu"];
        _InventoryOpenAction = PlayerInput.actions["Inventory"];
        _UIMenuCloseAction = PlayerInput.actions["MenuCloseAction"];
        _UIInventoryCloseAction = PlayerInput.actions["InventoryClose"];
    }

    private void UpdateInputs()
    {
        MenuOpenCloseInput = _menuOpenCloseAction.WasPressedThisFrame();
        UIMenuCloseInput = _UIMenuCloseAction.WasPressedThisFrame();
        InventoryOpen = _InventoryOpenAction.WasPressedThisFrame();
        InventoryClose = _UIInventoryCloseAction.WasPressedThisFrame();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputs();
    }
}
