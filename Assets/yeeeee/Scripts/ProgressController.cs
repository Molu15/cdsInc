using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ProgressController : MonoBehaviour
{
    public TMP_Text counterText;


    // Update is called once per frame
    void Update()
    {
        if (counterText != null)
        {
            counterText.text = $"Progress: {CheckProgress()} / {3}";
        }
        if (CheckProgress() == 3)
        {
            SceneManager.LoadScene("CongratsScene");
        }
    }

    private int CheckProgress ()
    {
        int counter = 0;
        if (GameProgressManager.Instance.GetFlappyCatProgress())
        {
            counter++;
        }
        if (GameProgressManager.Instance.GetToyHuntProgress())
        {
            counter++;
        }
        if (GameProgressManager.Instance.GetMemoryProgress())
        {
            counter++;
        }
        return counter;
    }
}
