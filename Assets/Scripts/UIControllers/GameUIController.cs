using UnityEngine;
using UnityEngine.UIElements;

class GameUIController : MonoBehaviour
{
    private VisualElement root;  // Root element of the UI Document

    private void OnEnable()
    {
        // Get the root of the visual tree from the UIDocument component
        root = GetComponent<UIDocument>().rootVisualElement;

        Debug.Log(root);
        // Find the button by its name
        VisualElement readyButton = root.Q<VisualElement>("ReadyButton");

        //PrintAllElements(root);

        // Add a click event listener to the button
        if (readyButton != null)
        {
            readyButton.RegisterCallback<ClickEvent>(ev => OnReadyButtonClick());
        }
        


    }
 
    private void OnReadyButtonClick()
    {
        Debug.Log("READY");
        DisableReadyButton();
        //this.OnDisable();
        GameManager.instance.InGame();
    }

   /* private void OnDisable()
    {
        DisableReadyButton();
    }*/
    public void DisableReadyButton()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        Debug.Log(root);
        // Find the button by its name
        VisualElement readyButton = root.Q<VisualElement>("ReadyButton");

        // Add a click event listener to the button
        if (readyButton != null)
        {
            readyButton.SetEnabled(false);
        }

    }

    public void EnableReadyButton()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        Debug.Log(root);
        // Find the button by its name
        VisualElement readyButton = root.Q<VisualElement>("ReadyButton");

        // Add a click event listener to the button
        if (readyButton != null)
        {
            readyButton.SetEnabled(true);
        }

    }

}