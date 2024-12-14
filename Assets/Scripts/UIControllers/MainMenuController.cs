using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

class MainMenuController : MonoBehaviour
{
    private VisualElement root;  // Root element of the UI Document
    private Label skipLabel;
    private Coroutine showLabelCoroutine;
    public AudioSource audioSource;
    [SerializeField] private AudioClip click;
    private void OnEnable()
    {
        // Get the root of the visual tree from the UIDocument component
        root = GetComponent<UIDocument>().rootVisualElement;

        Debug.Log(root);
        // Find the button by its name
        VisualElement startGameButton = root.Q<VisualElement>("startGameButton");
        VisualElement startIntroButton = root.Q<VisualElement>("startIntroButton");
        VisualElement settingsButton = root.Q<VisualElement>("SettingsButton");
        Label highScoreLabel = root.Q<Label>("ScoreAmtLabel");
        VisualElement quitButton = root.Q<VisualElement>("QuitButton");
        
        skipLabel = root.Q<Label>("skipLabel");

        //PrintAllElements(root);

        // Add a click event listener to the button
        if (startGameButton != null)
        {
            startGameButton.RegisterCallback<ClickEvent>(ev => OnStartGameButtonClick());
            startGameButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            startGameButton.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }
        if (startIntroButton != null)
        {
            startIntroButton.RegisterCallback<ClickEvent>(ev => OnStartIntroButtonClick());
        }
        if (settingsButton != null)
        {
            settingsButton.RegisterCallback<ClickEvent>(ev => OnSettingsButtonClick());
        }

        if (highScoreLabel != null)
        {
            highScoreLabel.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        }

        if (quitButton != null)
        {
            quitButton.RegisterCallback<ClickEvent>(ev => OnQuitButtonClick());
        }
        skipLabel.style.display = DisplayStyle.None;
    }
    
    private void OnMouseEnter(MouseEnterEvent evt)
    {
        // Start the coroutine to show the label after 0.5 seconds
        showLabelCoroutine = StartCoroutine(ShowLabelAfterDelay(evt.mousePosition));
    }

    private void OnMouseLeave(MouseLeaveEvent evt)
    {
        // Stop the coroutine if the mouse leaves before the delay is over
        if (showLabelCoroutine != null)
        {
            StopCoroutine(showLabelCoroutine);
            showLabelCoroutine = null;
        }

        // Hide the label immediately
        skipLabel.style.display = DisplayStyle.None;
    }
    
    private IEnumerator ShowLabelAfterDelay(Vector2 mousePosition)
    {
        // Wait for 0.5 seconds
        yield return new WaitForSeconds(0.5f);

        // Position the label at the mouse position
        skipLabel.style.left = mousePosition.x;
        skipLabel.style.top = mousePosition.y;

        // Show the label
        skipLabel.style.display = DisplayStyle.Flex;
    }
 
    private void OnStartGameButtonClick()
    {
        audioSource.PlayOneShot(click);
        MainMenuGameManager.instance.Play();
    }
    private void OnStartIntroButtonClick()
    {
        audioSource.PlayOneShot(click);
        MainMenuGameManager.instance.Intro();
    }
    private void OnSettingsButtonClick()
    {
        audioSource.PlayOneShot(click);
        MainMenuGameManager.instance.Settings();
    }
    private void OnQuitButtonClick()
    {
        // If the application is running in the editor, stop playing
        #if UNITY_EDITOR
              UnityEditor.EditorApplication.isPlaying = false;
        #else // If the application is running as a standalone build, quit the application
              Application.Quit();
        #endif
    }
    
}