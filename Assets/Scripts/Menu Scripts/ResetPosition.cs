using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;


public class ResetPosition : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textMeshProUGUI;
    public PersistenceManager manager;
    public MenuManager menuManager;
    public float timer = 60f;
    float delay = 60f;

    public void resetPlayerPosition()
    {
        if(timer <= 0)
        {
            menuManager.Unpause();
            manager.SetPlayerToSpawn();
            timer = delay;
        }
        
    }



    // Start is called before the first frame update
    void Start()
    {
        timer = delay;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            textMeshProUGUI.SetText("Reset Unavailable");
        }
        else
        {
            textMeshProUGUI.SetText("Reset Position");
        }
    }
}
