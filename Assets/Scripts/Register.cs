using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
    public InputField username;
    public InputField password;
    public Button registerButton;
    public Button gotToLoginButton;

    ArrayList credentials;

    void Start()
    {
        registerButton.onClick.AddListener(writeToFile);
        gotToLoginButton.onClick.AddListener(goToLoginScene);

        if(File.Exists(Application.dataPath + "/credentials.txt"))
        {
            credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.txt"));
        }
        else
        {
            File.WriteAllText(Application.dataPath + "/credentials.txt", "");
        }
    }

    void goToLoginScene()
    {
        SceneManager.LoadScene("Login");
    }

    void writeToFile()
    {
        bool exists = false;
        credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.txt"));
        foreach(var i in credentials)
        {
            if(i.ToString().Contains(username.text))
            {
                exists = true;
                break;
            }
        }
        if(exists)
        {
            Debug.Log($"Username '{username.text}' already exists!");
        }
        else
        {
            credentials.Add(username.text + ":" + password.text);
            File.WriteAllLines(Application.dataPath + "/credentials.txt", (String[])credentials.ToArray(typeof(String)));
            Debug.Log("Account Registered");
        }
    }
}
