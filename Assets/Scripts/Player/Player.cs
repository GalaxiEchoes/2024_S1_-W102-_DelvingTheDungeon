using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PersistenceManager;


[Serializable]
public class Player : MonoBehaviour
{
    public int money;
    public int health;
    public float stamina;
    public int attack;
    public int defense;
    public int maxHealth = 100;
    public float maxStamina = 100;

    public HealthBar healthBar;
    private InventoryHolder inventoryHolder;
    public MoneyTracker moneyTracker;

    private void Start()
    {
        money = 50;
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        stamina = maxStamina; 
        inventoryHolder = GetComponent<InventoryHolder>();
        moneyTracker.setMoney(money);
    }

    private void Update()
    {
        //On Death
        if (health <= 0)
        {
            Cursor.lockState = CursorLockMode.None;

            //Remove items from inventory
            List<ISlot> slots = inventoryHolder.EquipmentInventorySystem.EquipmentSlots;
            foreach (ISlot slot in slots)
            {
                if(slot.ItemData != null)
                {
                    minusStats(slot.ItemData.healthEffect, slot.ItemData.staminaEffect, slot.ItemData.attackEffect, slot.ItemData.defenseEffect);
                }
            }

            SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
        }

        if(Input.GetKeyDown("l"))

        {
            takeDamage(20);
        }

        moneyTracker.setMoney(money);

        PersistenceManager.OnLevelChanged += OnLevelChanged;
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
        health = maxHealth;

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

    private void OnLevelChanged(int newLevel)
    {
        // Increase money when the level changes
        money += 20;
    }
}
