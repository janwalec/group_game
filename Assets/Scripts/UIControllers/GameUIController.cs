using UnityEngine;
using UnityEngine.UIElements;

class GameUIController : MonoBehaviour
{
    private VisualElement root;  // Root element of the UI Document
    Label goldAmount;
    Label roundNumber;
    Button pauseButton;
    VisualElement readyButton;


    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        goldAmount = root.Q<Label>("GoldAmountLabel");
        roundNumber = root.Q<Label>("RoundNumber");
        readyButton = root.Q<VisualElement>("ReadyButton");
        pauseButton = root.Q<Button>("PauseButton");

        // Add a click event listener to the button
        if (readyButton != null)
        {
            readyButton.RegisterCallback<ClickEvent>(ev => OnReadyButtonClick());
        }
        if (pauseButton != null)
        {
            pauseButton.RegisterCallback<ClickEvent>(ev => OnPauseButtonClick());
        }


    }
 
    private void OnReadyButtonClick()
    {
        Debug.Log("READY");
        DisableReadyButton();
        //this.OnDisable();
        GameManager.instance.InGame();
    }

    private void OnPauseButtonClick()
    {
        GameManager.instance.PauseMenu();
    }

    public void SetReadyButtonActive(bool active)
    {
        if (active) { EnableReadyButton(); }
        else { DisableReadyButton(); }
    }

    private void DisableReadyButton()
    {
        /*root = GetComponent<UIDocument>().rootVisualElement;

        Debug.Log(root);
        // Find the button by its name
        VisualElement readyButton = root.Q<VisualElement>("ReadyButton");
        */
        // Add a click event listener to the button
        if (readyButton != null)
        {
            readyButton.SetEnabled(false);
        }

    }

    private void EnableReadyButton()
    {
/*        root = GetComponent<UIDocument>().rootVisualElement;

        Debug.Log(root);
        // Find the button by its name
        VisualElement readyButton = root.Q<VisualElement>("ReadyButton");
*/
        // Add a click event listener to the button
        if (readyButton != null)
        {
            readyButton.SetEnabled(true);
        }

    }

    public void UpdateGoldAmount(int value)
    {
        goldAmount.text = value.ToString();
    }

    public void UpdateRound(int level, int wave)
    {
        roundNumber.text = (level+1).ToString() + "-" + (wave+1).ToString();
    }
}