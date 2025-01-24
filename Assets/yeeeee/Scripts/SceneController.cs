using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public void PlayAgain()
    {
        GameProgressManager.Instance.ResetProgress();
        Destroy(GameProgressManager.Instance);
        SceneManager.LoadScene("StartScene");
        
    }

    public void ExitGame()
    {
        // Exit the application or return to a main menu if you have one
        Application.Quit();
    }

    public void StopTime()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }
    }

    public void ContinueTime()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }
}
