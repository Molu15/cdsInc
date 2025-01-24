using UnityEngine;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    // Card visual states
    [Header("Card Sprites")]
    public Sprite hiddenSprite;     // Default hidden card image
    public Sprite revealedSprite;   // Image when card is revealed

    // Card identification
    [Header("Card Properties")]
    public int cardID;               // Unique identifier for matching
    public bool isRevealed = false;  // Current reveal state
    public bool isMatched = false;   // Has this card been matched?

    // Audio settings
    [Header("Sound Effects")]
    public AudioClip cardFlipSound;      // Sound when card is flipped
    public AudioClip cardMatchSound;     // Sound when cards are matched
    public AudioClip cardMismatchSound;  // Sound when cards don't match
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;     // Volume control for sound effects

    // References
    private Image cardImage;
    private Button cardButton;
    private AudioSource audioSource;

    void Awake()
    {
        // Get references to components
        cardImage = GetComponent<Image>();
        cardButton = GetComponent<Button>();

        // Setup audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = soundVolume;

        // Set initial card state
        ResetCard();
    }

    // Called when card is clicked
    public void OnCardClicked()
    {
        // Prevent interaction if already matched or revealed
        if (isMatched || isRevealed) return;
        if (GameManager.Instance.GetcanInteract())
        {
            // Reveal card and notify game manager
            RevealCard();
            GameManager.Instance.OnCardSelected(this);
        }
    }

    // Reveal card's actual image
    public void RevealCard()
    {
        if (isMatched) return;

        isRevealed = true;
        cardImage.sprite = revealedSprite;
        PlaySound(cardFlipSound);
    }

    // Hide card back to initial state
    public void HideCard()
    {
        if (isMatched) return;

        isRevealed = false;
        cardImage.sprite = hiddenSprite;
        PlaySound(cardFlipSound);
    }

    // Reset card completely
    public void ResetCard()
    {
        isRevealed = false;
        isMatched = false;
        cardImage.sprite = hiddenSprite;
    }

    // Mark card as permanently matched
    public void MarkAsMatched()
    {
        isMatched = true;
        cardButton.interactable = false;
        PlaySound(cardMatchSound);
    }

    // Play mismatch sound when cards don't match
    public void PlayMismatchSound()
    {
        PlaySound(cardMismatchSound);
    }

    // Helper method to play sounds
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}