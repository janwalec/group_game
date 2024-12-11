using UnityEngine;
using UnityEngine.SceneManagement;

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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // This method will be called whenever a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scene") // Check if the loaded scene is "Scene"
        {
            if (inGameUI == null)
            {
                inGameUI = FindObjectOfType<GameUIController>(); // Find the in-game UI in the scene
            }
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe when the object is destroyed
        }
    }
    private void OnEnable()
    {
        if (inGameUI == null) inGameUI = FindObjectOfType<GameUIController>();
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

    public void resetScore()
    {
        score = 0;
    }
}