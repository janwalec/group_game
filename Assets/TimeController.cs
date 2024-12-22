using UnityEngine;
using UnityEngine.UIElements;

public class TimeController : MonoBehaviour
{
    public UIDocument uiDocument; // Reference to the UIDocument component
    private Button fastForwardButton;
    public float normalSpeed = 1f;
    public float fastForwardSpeed = 2f;
    private bool isFastForwarding = false;

    void Start()
    {
        if (uiDocument != null)
        {
            // Get the root visual element of the UI
            var rootVisualElement = uiDocument.rootVisualElement;

            // Find the button with the name "#FastForward" (the '#' is optional, depending on your UXML)
            fastForwardButton = rootVisualElement.Q<Button>("FastForward");

            if (fastForwardButton != null)
            {
                // Add a click event listener to the button
                fastForwardButton.RegisterCallback<ClickEvent>(ev => ToggleFastForward());
                
                // Set the button text initially
                UpdateButtonText();
            }
            else
            {
                Debug.LogError("Button #FastForward not found in the UI.");
            }
        }
        else
        {
            Debug.LogError("UIDocument is not assigned.");
        }

        // Set the initial time scale
        Time.timeScale = normalSpeed;

        // Update the button visibility based on the initial game state
        //UpdateFFButtonVisibility();
    }

    

    void ToggleFastForward()
    {
        if (isFastForwarding)
        {
            Time.timeScale = normalSpeed;
        }
        else
        {
            Time.timeScale = fastForwardSpeed;
        }

        isFastForwarding = !isFastForwarding;

        // Update the button text based on the fast forward state
        UpdateButtonText();
    }

    void UpdateButtonText()
    {
        if (fastForwardButton != null)
        {
            // Change the button text based on whether the fast forward is on or off
            fastForwardButton.text = isFastForwarding ? ">" : ">>";
        }
    }

    void OnDestroy()
    {
        if (fastForwardButton != null)
        {
            // Remove the click event listener when this object is destroyed
            fastForwardButton.UnregisterCallback<ClickEvent>(ev => ToggleFastForward());
        }

        // Reset time scale when the object is destroyed
        Time.timeScale = normalSpeed;
    }
}
