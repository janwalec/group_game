using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

class GameLostUIController : MonoBehaviour
{
    private VisualElement root;  // Root element of the UI Document
    private Label scoreText;
    private Label currGoldAmt;
    private Label accGoldAmt;
    private Label waveNmbr;

    private void OnEnable()
    {
        // Get the root of the visual tree from the UIDocument component
        root = GetComponent<UIDocument>().rootVisualElement;

        Debug.Log(root);
        // Find the button by its name
        VisualElement playAgainButton = root.Q<VisualElement>("PlayAgainButton");
        VisualElement mainMenuButton = root.Q<VisualElement>("Button");
        // Retrieve the score from PlayerPrefs
        int score = PlayerPrefs.GetInt("PlayerScore", 0); // Default to 0 if not found
        scoreText = root.Q<Label>("ScoreAmtLabel");
        currGoldAmt = root.Q<Label>("CurrGoldAmt");
        accGoldAmt = root.Q<Label>("AccGoldAmt");
        waveNmbr = root.Q<Label>("WaveNmbr");
        
        // Update the scoreText with the retrieved score
        scoreText.text = score.ToString();
        currGoldAmt.text = score.ToString();
        accGoldAmt.text = score.ToString();
        waveNmbr.text = PlayerPrefs.GetInt("Level", 0) + "-" + PlayerPrefs.GetInt("Wave", 0);
        

        //PrintAllElements(root);

        // Add a click event listener to the button
        if (playAgainButton != null)
        {
            playAgainButton.RegisterCallback<ClickEvent>(ev => OnPlayAgainButtonClick());
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.RegisterCallback<ClickEvent>(ev => OnMainMenuClick());
        }
        
        checkForNewHighScore();

    }
 
    private void OnPlayAgainButtonClick()
    {
        GameManager.instance.PlayAgain();
    }

    private void OnMainMenuClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void checkForNewHighScore()
    {
        int score = PlayerPrefs.GetInt("PlayerScore", 0); // Default to 0 if not found
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }
}