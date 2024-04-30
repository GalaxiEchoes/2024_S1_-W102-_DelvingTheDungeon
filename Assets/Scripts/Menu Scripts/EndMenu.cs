using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    public void ExitToMain()
    {
        SceneManager.LoadScene(0);
        //plus logic to reset stats
    }

    public void Respawn()
    {
        SceneManager.LoadScene(1);
        //plus logic to reset stats
    }
}
