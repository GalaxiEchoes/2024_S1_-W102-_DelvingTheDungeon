using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PersistenceManager;

public class EndMenu : MonoBehaviour
{
    public int bossLevel = 5;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ExitToMain()
    {
        StartNewGame();
        SceneManager.LoadScene(0);
    }

    public void Respawn()
    {
        StartNewGame();
        SceneManager.LoadScene(1);
    }

    private void StartNewGame()
    {
        for (int currentLevel = 0; currentLevel < bossLevel; currentLevel++)
        {
            string directoryPath = Application.dataPath + "/Saves";
            string filePath = directoryPath + "/" + currentLevel + "world_state.json";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        string levelTrackerPath = Application.dataPath + "/Saves" + "/level_tracker.json";
        if (File.Exists(levelTrackerPath))
        {
            File.Delete(levelTrackerPath);
        }

        string levelTrackerPathMeta = Application.dataPath + "/Saves" + "/level_tracker.json.meta";
        if (File.Exists(levelTrackerPathMeta))
        {
            File.Delete(levelTrackerPathMeta);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ClearLoginStatus()
    {
        PlayerPrefs.DeleteKey("IsLoggedIn");
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        ClearLoginStatus();
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
        {
            ClearLoginStatus();
        }
    }
}
