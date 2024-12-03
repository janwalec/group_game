using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

class PauseUIController : MonoBehaviour
{
    private VisualElement root;  // Root element of the UI Document

    private void OnEnable()
    {        
        // Get the root of the visual tree from the UIDocument component
        root = GetComponent<UIDocument>().rootVisualElement;

        Debug.Log(root);
        // Find the button by its name
        VisualElement resumeButton = root.Q<VisualElement>("ResumeButton");
        VisualElement settingsButton = root.Q<VisualElement>("SettingsButton");
        VisualElement mainMenuButton = root.Q<VisualElement>("Button");
        //PrintAllElements(root);

        // Add a click event listener to the button
        if (resumeButton != null)
        {
            resumeButton.RegisterCallback<ClickEvent>(ev => OnResumeButtonClick());
        }
        if (settingsButton != null)
        {
            settingsButton.RegisterCallback<ClickEvent>(ev => OnSettingsButtonClick());
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.RegisterCallback<ClickEvent>(evt => OnMainMenuClick());
        }


    }
 
    private void OnResumeButtonClick()
    {
        Debug.Log("RESUME");
        GameManager.instance.SetPreviousState();
    }
    private void OnSettingsButtonClick()
    {
        GameManager.instance.Settings();
    }
    
    private void OnMainMenuClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}