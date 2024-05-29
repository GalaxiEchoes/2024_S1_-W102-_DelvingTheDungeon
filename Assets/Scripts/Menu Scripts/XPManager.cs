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

    [SerializeField] AnimationCurve XPCurve;

    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Image XPFill;

    void Start()
    {
        updateLevel();
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
}
