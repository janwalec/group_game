using UnityEngine;
using UnityEngine.UIElements;

class GameLostUIController : MonoBehaviour
{
    private VisualElement root;  // Root element of the UI Document

    private void OnEnable()
    {
        // Get the root of the visual tree from the UIDocument component
        root = GetComponent<UIDocument>().rootVisualElement;

        Debug.Log(root);
        // Find the button by its name
        VisualElement playAgainButton = root.Q<VisualElement>("PlayAgainButton");

        //PrintAllElements(root);

        // Add a click event listener to the button
        if (playAgainButton != null)
        {
            playAgainButton.RegisterCallback<ClickEvent>(ev => OnPlayAgainButtonClick());
        }
        


    }
 
    private void OnPlayAgainButtonClick()
    {
        GameLostManager.instance.Play();
    }
    
}