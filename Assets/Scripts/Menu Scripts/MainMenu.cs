using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    PersistenceManager PersistenceManager;

    private void Start()
    {
        PersistenceManager = GetComponent<PersistenceManager>();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartNewGame()
    {
        PersistenceManager.StartNewGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Debug.Log("Game Ended");
        Application.Quit();
    }
}
