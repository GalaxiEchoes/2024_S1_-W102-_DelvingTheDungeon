using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class PlayerStatsData : MonoBehaviour
{
    // Print stats relating to item
    public TextMeshProUGUI health;
    public TextMeshProUGUI stamina;
    public TextMeshProUGUI attack;
    public TextMeshProUGUI defense;

    // Player who holds stats
    public Player assignedPlayer;

    private void Awake()
    {
        health.text = "Health ";
        stamina.text = "Stamina ";
        attack.text = "Attack ";
        defense.text = "Defense ";
    }

    // Update text fields to reflect players stats
    private void Update()
    {
        health.text = "Health " + assignedPlayer.health.ToString();
        stamina.text = "Stamina " + assignedPlayer.stamina.ToString("F0");
        attack.text = "Attack " + assignedPlayer.attack.ToString();
        defense.text = "Defense " + assignedPlayer.defense.ToString();
    }
}
