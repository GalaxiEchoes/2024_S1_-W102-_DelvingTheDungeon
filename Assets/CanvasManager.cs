using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject Register;
    public GameObject Login;
    public GameObject MainMenu;

    public void OnLoginButtonClick()
    {
        SwitchToLogin();
    } 

    public void OnRegisterButtonClick()
    {
        SwitchToRegister();
    }

    public void OnMenuButtonClick()
    {
        SwitchToMain();
    }

    private void SwitchToLogin()
    {
        Login.SetActive(true);
        Register.SetActive(false);
        MainMenu.SetActive(false);
    }

    private void SwitchToRegister()
    {
        Login.SetActive(false);
        Register.SetActive(true);
        MainMenu.SetActive(false);
    }

    private void SwitchToMain()
    {
        Login.SetActive(false);
        Register.SetActive(false);
        MainMenu.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        Register.SetActive(true);
        Login.SetActive(false);
        MainMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
