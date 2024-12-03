using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score;
    [SerializeField] private GameUIController inGameUI;
    
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        // Subscribe to the event
        GameEvents.OnEnemyKilled += AddScore;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event
        GameEvents.OnEnemyKilled -= AddScore;
    }
    
    public int Score
    {
        get => score;
        set
        {
            score = value;
            PlayerPrefs.SetInt("PlayerScore", score); // Save score to PlayerPrefs
            PlayerPrefs.Save(); // Ensure the data is written to disk
        }
    }

    private void AddScore(int points)
    {
        // Update the score
        Score += points;
        if (inGameUI != null)
        {
            inGameUI.UpdatePoints(score);
        }
    }
}