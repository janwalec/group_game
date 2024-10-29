using UnityEngine;
using UnityEngine.UIElements;

class MainMenuController : MonoBehaviour
{
    private VisualElement root;  // Root element of the UI Document

    private void OnEnable()
    {
        // Get the root of the visual tree from the UIDocument component
        root = GetComponent<UIDocument>().rootVisualElement;

        Debug.Log(root);
        // Find the button by its name
        VisualElement startGameButton = root.Q<VisualElement>("startGameButton");
        VisualElement startIntroButton = root.Q<VisualElement>("startIntroButton");

        //PrintAllElements(root);

        // Add a click event listener to the button
        if (startGameButton != null)
        {
            startGameButton.RegisterCallback<ClickEvent>(ev => OnStartGameButtonClick());
        }
        if (startIntroButton != null)
        {
            startIntroButton.RegisterCallback<ClickEvent>(ev => OnStartIntroButtonClick());
        }


    }
 
    private void OnStartGameButtonClick()
    {
        MainMenuGameManager.instance.Play();
    }
    private void OnStartIntroButtonClick()
    {
        MainMenuGameManager.instance.Intro();
    }
}