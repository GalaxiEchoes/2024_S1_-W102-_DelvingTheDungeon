//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PersistenceManager;

public class EndMenu : MonoBehaviour
{
    public int bossLevel = 5;
    PersistenceManager manager;

    public void Start()
    {
        manager = GetComponent<PersistenceManager>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitToMain()
    {
        manager.StartNewRun();
        SceneManager.LoadScene(0);
    }

    public void Respawn()
    {
        manager.StartNewRun();
        SceneManager.LoadScene(1);
    }
}
