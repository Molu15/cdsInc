using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    [Header("Game Configuration")]
    public MemoryCard[] allCards;          // All cards in the game
    public Sprite[] cardFrontSprites;      // Different card front images
    public int gridRows = 4;               // Number of rows
    public int gridColumns = 4;            // Number of columns

    [Header("Timer Configuration")]        // Timer in the game
    public float initialGameTime = 300f;   // 2,5 minutes game time
    public bool timerIsRunning = false;    // Timer-Status
    private float currentGameTime;        // current game time
    private bool isGameRunning = true;     // stop timer when game ends

    [Header("UI Elements")]
    public TMPro.TextMeshProUGUI timerText;           // Display remaining time
    public TMPro.TextMeshProUGUI moveCounterText;    // Display number of moves
    public GameObject explain;           // Reference to explanation panel
    public GameObject helpButton;
    public GameObject helpText;
    public Button startButton;

    [Header("Win & Loose")]
    public GameObject winPanel;      // Neues Win-Panel
    public GameObject losePanel;     // Neues Lose-Panel 
    public TMPro.TextMeshProUGUI winText;    // Text im Win-Panel
    public TMPro.TextMeshProUGUI loseText;   // Text im Lose-Panel

    [Header("Sound Effects")]
    public AudioSource matchSound;         // Sound when cards match
    public AudioSource mismatchSound;      // Sound when cards don't match
    public AudioSource backgroundMusic;    // Background game music

    [Header("Game Parameters")]
    public float gameDuration = 150f;      // 2,5 minutes game time
    public int maxAllowedMoves = 50;       // Maximum moves allowed

    // Internal game state tracking
    private float currentTime;
    private int moveCount;
    private List<MemoryCard> selectedCards = new List<MemoryCard>();
    private bool canInteract = true;
    private bool isActive = false;
    private bool isPaused = false;



    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeGame();
    }

    void Start()
    {
        // Debugging
        Debug.Log("StartGame called.");
        // Debug: delete all  PlayerPrefs-Daten
        //PlayerPrefs.DeleteAll();  // <-- Diese Zeile hinzufügen
        Debug.Log("in  start");
        if (PlayerPrefs.GetInt("HasPlayedMemoryBefore") == 0)
        {
            // First time playing, show explanation panel
            Debug.Log("neu");
            explain.SetActive(true);
            helpButton.gameObject.SetActive(false);
            
            timerText.gameObject.SetActive(false);
            moveCounterText.gameObject.SetActive(false);
        }
        else
        {
            // Not the first time, start game directly
            Debug.Log("direkt start");
            StartGame();
        }
    }

    public void StartGame()
    {
        isActive = true;
        // Set PlayerPrefs to indicate the game has been played
        if (explain.activeSelf)
        {PlayerPrefs.SetInt("HasPlayedMemoryBefore", 1);
        PlayerPrefs.Save(); }
        

        // Hide explanation panel if it's active
        if (explain != null)
        {
            explain.SetActive(false);
        }

        // Show game UI
        timerText.gameObject.SetActive(true);
        moveCounterText.gameObject.SetActive(true);
        if (helpButton != null)
        {
            helpButton.gameObject.SetActive(true);
        }

        // Initialize game parameters
        currentTime = gameDuration;
        UpdateTimerDisplay();
        UpdateMoveCounter();
    }

    public void ShowHelp()
    {
        isPaused = true;
        helpText.SetActive(true);
        helpButton.gameObject.SetActive(false);
        isGameRunning = false; // Stops the timer
    }

    public void ContinueGame()
    {
        isPaused = false;
        helpText.SetActive(false);
        helpButton.gameObject.SetActive(true);
        isGameRunning = true; // Resumes the timer
    }

    // Start from explanation view
    public void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked");
        StartGame();
    }

    void Update()
    {
        if (isActive && !isPaused)
        //if (isActive)
        {
            UpdateGameTimer();
        }
    }


 

    void InitializeGame()
    {
        
        // Create list of card pairs
        Debug.Log("initialize");
        List<int> cardPairs = new List<int>();
        for (int i = 0; i < (gridRows * gridColumns) / 2; i++)
        {
            cardPairs.Add(i);
            cardPairs.Add(i);
        }
        Debug.Log(cardPairs);
        // Randomize card assignments
        for (int i = 0; i < allCards.Length; i++)
        {
            int randomIndex = Random.Range(0, cardPairs.Count-1);
            allCards[i].cardID = cardPairs[randomIndex];
            allCards[i].revealedSprite = cardFrontSprites[cardPairs[randomIndex]];
            cardPairs.RemoveAt(randomIndex);
        }
        Debug.Log(allCards.Length);
    }

    public void OnCardSelected(MemoryCard card)
    {
        
        if (!canInteract) return;
       
        // Prevent selecting same card twice
        if (selectedCards.Contains(card)) return;

        selectedCards.Add(card);
     
        IncrementMoveCounter();
       
        if (selectedCards.Count == 2)
        {
            StartCoroutine(CheckCardMatch());
        }
      
    }

    IEnumerator CheckCardMatch()
    {
        canInteract = false;
        Debug.Log("match 1");
        yield return new WaitForSeconds(1f);
        Debug.Log("match 2");
        bool isMatch = selectedCards[0].cardID == selectedCards[1].cardID;
        Debug.Log("match 3");

        if (isMatch)
        {
            // Match found
            matchSound.Play();
            selectedCards[0].MarkAsMatched();
            selectedCards[1].MarkAsMatched();
        }
        else
        {
            // No match
            mismatchSound.Play();
            selectedCards[0].HideCard();
            selectedCards[1].HideCard();
        }
        Debug.Log("match 4");
        selectedCards.Clear();
        canInteract = true;

        CheckGameCompletion();
    }

    void UpdateGameTimer()
    {
        Debug.Log("updategametimer");
        if (!isGameRunning) return;  // if game is done timer stops

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            EndGame(false);
        }
        else
        {
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    void IncrementMoveCounter()
    {
        Debug.Log("incrementmovecounter");
        moveCount++;
        UpdateMoveCounter();

        if (moveCount >= maxAllowedMoves)
        {
            EndGame(false);
        }
    }

    void UpdateMoveCounter()
    {
        moveCounterText.text = "Moves: " + moveCount;
    }

    void CheckGameCompletion()
    {
        Debug.Log("Checkgamecompletion");
        bool allMatched = true;
        foreach (MemoryCard card in allCards)
        {
            if (!card.isMatched)
            {
                allMatched = false;
                break;
            }
        }

        if (allMatched)
        {
            EndGame(true);
            GameProgressManager.Instance.CompleteMemoryGame();
        }
    }

    void EndGame(bool playerWon)
    {
        canInteract = false;
        isGameRunning = false;

        if (playerWon)
        {
            float timeUsed = gameDuration - currentTime;
            int minutesUsed = Mathf.FloorToInt(timeUsed / 60);
            int secondsUsed = Mathf.FloorToInt(timeUsed % 60);

            string winMessage = string.Format("Congratulations!\n\nTime used: {0:00}:{1:00}\nMoves needed: {2}",
                minutesUsed, secondsUsed, moveCount);

            winPanel.SetActive(true); // show congratulation message
            winText.SetText(winMessage);
            winPanel.GetComponent<AudioSource>().Play();
        }
        else
        {
            losePanel.SetActive(true); //show lose message
            loseText.SetText("Game Over! Try Again!");
            losePanel.GetComponent<AudioSource>().Play();
        }
    }

    public void RestartGame()
    {
        // activate timer again
		isGameRunning = true;



        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool GetcanInteract ()
    {
        return canInteract;
    }
}