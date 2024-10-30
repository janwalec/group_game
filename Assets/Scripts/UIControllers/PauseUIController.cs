using UnityEngine;
using UnityEngine.UIElements;

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

        //PrintAllElements(root);

        // Add a click event listener to the button
        if (resumeButton != null)
        {
            resumeButton.RegisterCallback<ClickEvent>(ev => OnResumeButtonClick());
        }
        


    }
 
    private void OnResumeButtonClick()
    {
        Debug.Log("RESUME");
        GameManager.instance.InGame();
    }
    
}