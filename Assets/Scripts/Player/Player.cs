using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[Serializable]
public class Player : MonoBehaviour, IDamageable
{
    public int money = 50;
    public int health = 100;
    public float stamina;
    public int attack;
    public int defense;
    public int maxHealth;
    public float maxStamina;

    public MoneyTracker moneyTracker;
    public HealthBar healthBar;
    private InventoryHolder inventoryHolder;

    public XPManager xpManager;

    private void Start()
    {
        if (moneyTracker != null)
        {
            moneyTracker.setMoney(money);
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(health);
        }

        if(inventoryHolder == null)
        {
            inventoryHolder = GetComponent<InventoryHolder>();
        }

        if(xpManager == null)
        {
            xpManager = GetComponent<XPManager>();
        }
    }

    private void Update()
    {
        moneyTracker.setMoney(money);

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
            Damage(20);
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

    public void increaseStats(int increaseHealth, int increaseStamina, int increaseAttack, int increaseDefense)
    {
        maxHealth += increaseHealth;
        maxStamina += increaseStamina;
        stamina = maxStamina;
        attack += increaseAttack;
        defense += increaseDefense;

    }

    public void resetHealth()
    {
        health = maxHealth;

    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;

        healthBar.SetHealth(health);
    }

    public void addMoney(int amount)
    {
        money += amount;
    }

    public void minusMoney(int amount)
    {
        money -= amount;
    }

    public void gainXP(int amount)
    {
        if(xpManager != null)
        {
            xpManager.addXP(amount);
        }
    }

    public void gainMoney(int amount)
    {
        addMoney(amount);
    }
}
