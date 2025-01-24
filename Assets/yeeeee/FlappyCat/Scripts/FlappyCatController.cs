using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FlappyCatController : MonoBehaviour
{
    public static FlappyCatController Instance;
    private GameObject player;
    public GameObject gameOverScreen; // Assign in the Inspector
    public bool active = false;
    public TMP_Text counterText;
    public int maxObstacles = 4;
    private int counter = 0;
    public GameObject explain;
    public GameObject congrats;
    public Button backButton;
    public Button helpButton;
    private TiltControl tiltController;

    public TMP_Text debug;

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;

            if (PlayerPrefs.GetInt("HasPlayedFlappyBefore", 0) == 0)
            {
                // First time playing, show explanation panel
                explain.SetActive(true);
                helpButton.gameObject.SetActive(false);
                counterText.gameObject.SetActive(false);
            }
            else
            {
                // Not the first time, disable explanation panel
                StartGame();
            }

            if (counterText != null)
            {
                counterText.text = $"Progress: {counter} / {maxObstacles}";
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        tiltController = GetComponent<TiltControl>();
    }

    public void StartGame()
    {
        active = true;
        counterText.gameObject.SetActive(true);
        GetComponent<AudioSource>().PlayDelayed(1f);
        if (explain.activeSelf)
        {
            PlayerPrefs.SetInt("HasPlayedFlappyBefore", 1);
            PlayerPrefs.Save();
        }
        backButton.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        // Display the Game Over screen
        gameOverScreen.SetActive(true);
        gameOverScreen.GetComponent<AudioSource>().Play();
        active = false;

        // Optionally, stop obstacle spawning or player movement
        Time.timeScale = 0; // Freeze game (optional)
    }

    public void RestartGame()
    {
        // Reset game (Restart the current scene)
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1; // Unfreeze time
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        counter = 0;
        congrats.SetActive(false);
    }

    private void ShowEndGamePanel()
    {
        active = false;
        congrats.SetActive(true);
        congrats.GetComponent<AudioSource>().Play();
        backButton.gameObject.SetActive(false);
        helpButton.gameObject.SetActive(false);
        counterText.gameObject.SetActive(false);
        GameProgressManager.Instance.CompleteFlappyCatGame();
    }

    public void CloseGame()
    {
        GameProgressManager.Instance.GoToHome(); 
        Destroy(gameObject);
    }

    public void FinishGame (GameObject prefab)
    {
        if (prefab == null)
        {
            ShowEndGamePanel();
            helpButton.gameObject.SetActive(false);
        }
    }

    public void UpdateCounter()
    {
        counter++;
        if (counter % 2 == 0 && counter / 2 <= maxObstacles)
        {
            if (counterText != null)
            {
                counterText.text = $"Progress: {counter / 2} / {maxObstacles}";
            }
        }

    }

    public void setPlayer(GameObject prefab)
    {
        player = prefab;
    }

    public GameObject getPlayer ()
    {
        return player;
    }

    public void StopTime ()
    {
        if (Time.timeScale == 1 && active)
        {
            Time.timeScale = 0;
        }
    }

    public void ContinueTime ()
    {
        if (Time.timeScale == 0 && active)
        {
            Time.timeScale = 1;
        }
    }
}


