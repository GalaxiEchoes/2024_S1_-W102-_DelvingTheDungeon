using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    public void ExitToMain()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>().DeleteGame();
        SceneManager.LoadScene(0);
        //plus logic to reset stats
    }

    public void Respawn()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>().DeleteGame();
        SceneManager.LoadScene(1);
        //plus logic to reset stats
    }
}
