using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PersistenceManager;

public class Player : MonoBehaviour
{
    public int health;
    public int stamina;
    public int attack;
    public int defense;
    public int bossLevel = 5;

    private void Update()
    {
        if (health <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            ResetGame();
            SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
        }
    }

    public void addStats(int _health, int _stamina, int _attack, int _defense)
    {
        health += _health;
        stamina += _stamina;
        attack += _attack;
        defense += _defense;
    }

    public void minusStats(int _health, int _stamina, int _attack, int _defense)
    {
        health -= _health;
        stamina -= _stamina;
        attack -= _attack;
        defense -= _defense;
    }

    public void resetHealth()
    {
        health = 100;
    }

    private void ResetGame()
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
    }
}
