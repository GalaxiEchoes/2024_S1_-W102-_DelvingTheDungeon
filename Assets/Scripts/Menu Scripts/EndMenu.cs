using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PersistenceManager;

public class EndMenu : MonoBehaviour
{
    public int bossLevel = 5;

    public void ExitToMain()
    {
        StartNewGame();
        SceneManager.LoadScene(0);
        //plus logic to reset stats
    }

    public void Respawn()
    {
        StartNewGame();
        SceneManager.LoadScene(1);
        //plus logic to reset stats
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
