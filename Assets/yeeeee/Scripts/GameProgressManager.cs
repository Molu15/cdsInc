using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    private bool memoryGameCompleted = false;
    private bool toyHuntGameCompleted = false;
    private bool flappyCatGameCompleted = false;

    public bool popupShown = false;


    private void Start ()
    {
        PlayerPrefs.DeleteAll();
        this.GetComponent<AudioSource>().Play();
    }

    private void Awake()
    {        

        // Find all existing instances
        GameProgressManager[] instances = FindObjectsOfType<GameProgressManager>();

        // Singleton logic with extensive logging
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void GoToHome ()
    {
        SceneManager.LoadScene("Home");
    }

    public void CompleteMemoryGame()
    {
        memoryGameCompleted = true;
    }

    public void CompleteToyHuntGame()
    {
        toyHuntGameCompleted = true;
    }

    public void CompleteFlappyCatGame()
    {
        flappyCatGameCompleted = true;
        
    }

    public bool CheckAllGamesCompleted()
    {
        if (memoryGameCompleted && toyHuntGameCompleted && flappyCatGameCompleted)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetProgress()
    {
        memoryGameCompleted = false;
        toyHuntGameCompleted = false;
        flappyCatGameCompleted = false;
    }

    public void StartMinigame(string markerName)
    {
        if (markerName == "MemoryMarker")
        {
            SceneManager.LoadScene("MemoryGame");
        }
        else if (markerName == "ToyHuntMarker")
        {
            SceneManager.LoadScene("ToyHuntGame");
        }
        else if (markerName == "FlappyCatMarker")
        {
            SceneManager.LoadScene("FlappyCatGame");
        }
    }

    public bool GetFlappyCatProgress ()
    {
        return flappyCatGameCompleted;
    }

    public bool GetToyHuntProgress ()
    {
        return toyHuntGameCompleted;
    }

    public bool GetMemoryProgress ()
    {
        return memoryGameCompleted;
    }
}
