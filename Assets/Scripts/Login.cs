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
            Debug.Log($"Logging in '{username.text}'");
            loadWelcomeScreen();
        }
        else
        {
            Debug.Log("Incorrect credentials!");
        }
    }

    void moveToRegister()
    {
        SceneManager.LoadScene("Register");
    }

    void loadWelcomeScreen()
    {
        SceneManager.LoadScene("WelcomeScreen");
    }
}
