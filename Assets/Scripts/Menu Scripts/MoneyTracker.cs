using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyTracker : MonoBehaviour
{
    public TextMeshProUGUI moneyCount;

    public void setMoney(int money)
    {
        moneyCount.text = money.ToString();
    }
}
