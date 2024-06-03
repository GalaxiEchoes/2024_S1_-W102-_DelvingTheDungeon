using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyHealth : MonoBehaviour
{ 
    [SerializeField] public Slider slider;

    public void UpdateHealthBar(int currentValue, int maxValue)
    {
        slider.value = currentValue / maxValue;
    }
}
