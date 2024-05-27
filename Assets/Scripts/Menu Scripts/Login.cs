using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Login : MonoBehaviour
{
    /*public TMPro.TextMeshProUGUI text;
    public CanvasManager canvasManager;
    public TMP_InputField username;
    public TMP_InputField password;
    public Button loginButton;
    public Button goToRegisterButton;

    ArrayList credentials;

    // Start is called before the first frame update
    void Start()
    {
        loginButton.onClick.AddListener(login);
        goToRegisterButton.onClick.AddListener(moveToRegister);

        if(File.Exists(Application.dataPath + "/credentials.txt"))
        {
            credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.txt"));
        }
        else
        {
            Debug.Log("Credential file does not exist!");
        }
    }

    // Update is called once per frame
    void login()
    {
        bool exists = false;
        credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.txt"));

        foreach(var i in credentials)
        {
            string line = i.ToString();
            if(i.ToString().Substring(0, i.ToString().IndexOf(":")).Equals(username.text)
                && i.ToString().Substring(i.ToString().IndexOf(":") + 1).Equals(password.text))
            {
                exists = true;
                break;
            }
        }

        if(exists)
        {
            text.SetText("");
            Debug.Log($"Logging in '{username.text}'");
            canvasManager.SetLoggedIn(true);
            loadWelcomeScreen();
        }
        else
        {
            text.SetText("Incorrect credentials!");
        }
    }

    void moveToRegister()
    {
        canvasManager.OnRegisterButtonClick();
    }

    void loadWelcomeScreen()
    {
        canvasManager.OnMenuButtonClick();
    }*/
}
