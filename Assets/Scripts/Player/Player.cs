using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int money;
    public int health;
    public int stamina;
    public int attack;
    public int defense;
    public int maxHealth = 100;

    public HealthBar healthBar;
    public MoneyTracker moneyTracker;

    private void Start()
    {
        money = 50;
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        moneyTracker.setMoney(money);
    }

    private void Update()
    {
        if (health <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
        }

        if(Input.GetKeyDown("l"))
        {
            takeDamage(20);
        }

        moneyTracker.setMoney(money);
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

    void takeDamage(int damage)
    {
        health -= damage;

        healthBar.SetHealth(health);
    }

    public void addMoney(int amount)
    {
        money += amount;
    }
}
