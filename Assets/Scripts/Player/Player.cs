using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[Serializable]
public class Player : MonoBehaviour
{
    public int health;
    public float stamina;
    public int attack;
    public int defense;
    public int maxHealth;
    public float maxStamina;

    public HealthBar healthBar;
    private InventoryHolder inventoryHolder;

    private void Start()
    {
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(health);
        inventoryHolder = GetComponent<InventoryHolder>();
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
}