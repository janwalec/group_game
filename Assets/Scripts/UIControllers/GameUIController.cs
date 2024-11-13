using UnityEngine;
using UnityEngine.UIElements;

class GameUIController : MonoBehaviour
{
    private VisualElement root;  // Root element of the UI Document
    Label goldAmount;
    Label roundNumber;
    Button pauseButton;
    VisualElement readyButton;
    VisualElement shopButton;
    
    public AudioSource audioSource;
    [SerializeField] AudioClip onBattleStartSound;
    
    public UIDocument shopUIDocument;


    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        goldAmount = root.Q<Label>("GoldAmountLabel");
        roundNumber = root.Q<Label>("RoundNumber");
        readyButton = root.Q<VisualElement>("ReadyButton");
        pauseButton = root.Q<Button>("PauseButton");
        shopButton = root.Q<VisualElement>("GoldAmt");

        // Add a click event listener to the button
        if (readyButton != null)
        {
            readyButton.RegisterCallback<ClickEvent>(ev => OnReadyButtonClick());
        }
        if (pauseButton != null)
        {
            pauseButton.RegisterCallback<ClickEvent>(ev => OnPauseButtonClick());
        }

        if (shopButton != null)
        {
            shopButton.RegisterCallback<ClickEvent>(ev => OnShopButtonClick());
        }
        
        audioSource = GetComponent<AudioSource>();

    }
 
    private void OnReadyButtonClick()
    {
        Debug.Log("READY");
        DisableReadyButton();
        //this.OnDisable();
        audioSource.PlayOneShot(onBattleStartSound);
        
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


    private void OnShopButtonClick()
    {
        //shopUIDocument.SetActive(!shopUIDocument.activeSelf);
        shopUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }


    public void DisableReadyButton()
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
        //Debug.Log("updating " + level + wave);
        roundNumber.text = (level+1).ToString() + "-" + (wave+1).ToString();
    }
}