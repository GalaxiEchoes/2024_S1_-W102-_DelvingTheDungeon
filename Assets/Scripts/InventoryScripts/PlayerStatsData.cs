using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class PlayerStatsData : MonoBehaviour
{
    public TextMeshProUGUI Health;
    public TextMeshProUGUI Stamina;
    public TextMeshProUGUI Attack;
    public TextMeshProUGUI Defense;
    public Player AssignedPlayer;

    private void Awake()
    {
        Health.text = "Health ";
        Stamina.text = "Stamina ";
        Attack.text = "Attack ";
        Defense.text = "Defense ";
    }

    private void Update()
    {
        Health.text = "Health " + AssignedPlayer.health.ToString();
        Stamina.text = "Stamina " + AssignedPlayer.stamina.ToString("F0");
        Attack.text = "Attack " + AssignedPlayer.attack.ToString();
        Defense.text = "Defense " + AssignedPlayer.defense.ToString();
    }
}
