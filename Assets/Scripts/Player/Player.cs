using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[Serializable]
public class Player : MonoBehaviour, IDamageable
{
    public int health = 100;
    public float stamina;
    public int attack;
    public int defense;
    public int maxHealth;
    public float maxStamina;

    public HealthBar healthBar;
    private InventoryHolder inventoryHolder;

    private void Start()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(health);
        }
        if(inventoryHolder != null)
        {
            inventoryHolder = GetComponent<InventoryHolder>();
        }
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

    public void resetHealth()
    {
        health = maxHealth;

    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;

        healthBar.SetHealth(health);
    }
}
