using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public void Damage(int damageAmount)
    {
        Debug.Log("Damage: " + damageAmount);
    }
}
