using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int health;
    public int stamina;
    public int attack;
    public int defense;

    private void Update()
    {
        if (health <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
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
}
