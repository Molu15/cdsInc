using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToyHuntController : MonoBehaviour
{
    public static ToyHuntController Instance;
    public List<string> requiredToys = new List<string> { "Fish", "Feather" };
    public List<string> foundToys = new List<string>();
    private ARPlaneManager planeManager;
    public TextMeshProUGUI counterText;
    public GameObject congratsMsg;
    public GameObject backButton;
    public GameObject resetButton;
    public GameObject helpButton;
    public GameObject explainPanel;
    public bool active = false;
    private float congratsDelay = 0.5f;
    private float hideCounterDelay = 0.1f;

    public GameObject wrong;
    public GameObject right;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (PlayerPrefs.GetInt("HasPlayedToyBefore", 0) == 0)
            {
                explainPanel.SetActive(true);
                helpButton.SetActive(false);
                resetButton.SetActive(false);
                counterText.gameObject.SetActive(false);
            }
            else
            {
                explainPanel.SetActive(false);
                StartGame();
            }
            if (counterText != null)
            {
                counterText.text = $"Progress: {foundToys.Count} / {requiredToys.Count}";
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void HideCounter()
    {
        if (counterText != null)
        {
            counterText.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        active = true;

        if (explainPanel.activeSelf)
        {
            PlayerPrefs.SetInt("HasPlayedToyBefore", 1);
            PlayerPrefs.Save();
            explainPanel.SetActive(false);
        }

        helpButton.SetActive(true);
        backButton.SetActive(true);
        resetButton.SetActive(true);
        if (counterText != null)
        {
            counterText.gameObject.SetActive(true);
        }
        planeManager = GetComponent<ARPlaneManager>();
        UpdateCounter();
    }

    public void ShowHelpScreen()
    {
        //backButton.SetActive(false);
        resetButton.SetActive(false);
        helpButton.SetActive(false);
        Invoke("HideCounter", hideCounterDelay);
    }

    public void ContinueFromHelp()
    {
        //backButton.SetActive(true);
        resetButton.SetActive(true);
        helpButton.SetActive(true);
        if (counterText != null)
        {
            counterText.gameObject.SetActive(true);
        }
    }

    public bool IsToyRequired(string toyName)
    {
        if (requiredToys.Contains(toyName))
        {
            return true;
        }
        else
        {
            wrong.GetComponent<AudioSource>().Play();
            return false;
        }
    }

    public bool IsToyAlreadyFound(string toyName)
    {
        if (foundToys.Contains(toyName))
        {
            wrong.GetComponent<AudioSource>().Play();
            return true;
        }
        else
        {
            right.GetComponent<AudioSource>().Play();
            return false;
        }
    }

    public void ToyFound(string toyName)
    {
        if (requiredToys.Contains(toyName) && !foundToys.Contains(toyName))
        {
            foundToys.Add(toyName);
            UpdateCounter();
            if (foundToys.Count == requiredToys.Count)
            {
                ShowCompletionMessage();
            }
        }
    }

    private void UpdateCounter()
    {
        if (counterText != null)
        {
            counterText.text = $"Progress: {foundToys.Count} / {requiredToys.Count}";
        }
    }

    private void ShowCompletionMessage()
    {
        active = false;
        GameProgressManager.Instance.CompleteToyHuntGame();
        backButton.SetActive(false);
        resetButton.SetActive(false);
        helpButton.SetActive(false);
        Invoke("HideCounter", hideCounterDelay);
        Invoke("ShowCongrats", congratsDelay);
    }

    private void ShowCongrats()
    {
        congratsMsg.SetActive(true);
        congratsMsg.GetComponent<AudioSource>().Play();
    }

    public void ResetPlanes()
    {
        ToyItem[] allToys = FindObjectsOfType<ToyItem>();
        foreach (ToyItem toy in allToys)
        {
            Destroy(toy.gameObject);
        }
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        planeManager.enabled = false;
        planeManager.enabled = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CloseGame()
    {
        GameProgressManager.Instance.GoToHome();
        Destroy(gameObject);
    }
}