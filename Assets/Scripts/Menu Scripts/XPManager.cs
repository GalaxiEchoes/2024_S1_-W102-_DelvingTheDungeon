using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class XPManager : MonoBehaviour
{
    int currentLevel;
    int totalXP;
    int prevLevelXP;
    int nextLevelXP;

    int increaseHealth = 20;
    int increaseStamina = 10;
    int increaseAttack = 5;
    int increseDefense = 3;

    [SerializeField] AnimationCurve XPCurve;

    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Image XPFill;

    [SerializeField] Player player; // Reference to the Player class

    void Start()
    {
        updateLevel();
        updateInterface();
    }

    void Update()
    {
        if(Input.GetKeyUp("x")) //For testing
        {
            addXP(5);
        }
    }

    public void addXP(int amount)
    {
        totalXP += amount;
        checkForLevelUp();
        updateInterface();
    }

    void checkForLevelUp()
    {
        if(totalXP >= nextLevelXP)
        {
            currentLevel++;
            increaseStats();
            updateLevel();
        }
    }

    void updateLevel()
    {
        prevLevelXP = (int)XPCurve.Evaluate(currentLevel);
        nextLevelXP = (int)XPCurve.Evaluate(currentLevel + 1);
        updateInterface();
    }

    void updateInterface()
    {
        int start = totalXP - prevLevelXP;
        int end = nextLevelXP - prevLevelXP;

        levelText.text = currentLevel.ToString();
        XPFill.fillAmount = (float)start / (float)end; 
    }

    void increaseStats()
    {
        // Increasing player stats slightly upon leveling up:
        player.increaseStats(increaseHealth, increaseStamina, increaseAttack, increseDefense);
        player.resetHealth(); // Resetting health to max health

        if(player.healthBar != null)
        {
            player.healthBar.SetMaxHealth(player.maxHealth);
            player.healthBar.SetHealth(player.health);
        }
    }
}
