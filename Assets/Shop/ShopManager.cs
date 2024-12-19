using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;


public class ShopManager : MonoBehaviour
{
    public ItemCollection currentItemCollection;
    public VisualTreeAsset itemTemplate;
    private ScrollView shopScrollView;
    public UIDocument shopUIDocument; // Reference to the overall shop UI document
    private Button closeButton;
    
    public GameObject shop;

    public GameObject cannonCard;
    public GameObject diceCard;
    public GameObject coinCard;

    private TextMeshProUGUI cannonCardText;
    private TextMeshProUGUI diceCardText;

    private TextMeshProUGUI coinCardText;


    public int startingCannons, startingDice, startingCoins;
    public int cannonsBought = 0, diceBought = 0, coinsBought = 0;

    Label goldAmount;

    [SerializeField] private MarketManager marketManager;
    
    AudioSource audioSource;
    [SerializeField] AudioClip onBuySound;
    [SerializeField] private AudioClip onCloseSound;

    public void RestartShopManager() {
        Debug.Log("RESTARTING IN SHOP MANAGER");
        cannonsBought = startingCannons;
        diceBought = startingDice;
        coinsBought = startingCoins;
    }

    void initStarting() {
        if (startingCannons > 0) {
            cannonCard.SetActive(true);
            cannonsBought += startingCannons;
            this.cannonCardText.text = cannonsBought.ToString();
        }
        if (startingDice > 0) {
            diceCard.SetActive(true);
            diceBought += startingDice;
            this.diceCardText.text = diceBought.ToString();
        }
        if (startingCoins > 0) {
            coinCard.SetActive(true);
            coinsBought += startingCoins;
            this.coinCardText.text = coinsBought.ToString();
        }
    }

    void Awake() {
        audioSource = GetComponent<AudioSource>();

        cannonCard.SetActive(false);
        diceCard.SetActive(false);
        coinCard.SetActive(false);
        
        this.cannonCardText = cannonCard.GetComponentInChildren<TextMeshProUGUI>();
        this.diceCardText = diceCard.GetComponentInChildren<TextMeshProUGUI>();
        this.coinCardText = coinCard.GetComponentInChildren<TextMeshProUGUI>();
        

        GameManager.instance.setShopManager(this);
        initStarting();
        
    }


    void OnEnable()
    {
        

        // Reference to the root of the UI
        var root = shopUIDocument.rootVisualElement;
        shopScrollView = root.Q<ScrollView>("ShopScrollView");

        PopulateShop();
        
        //Setup xclose button
        closeButton = root.Q<Button>("XClose");
        if (closeButton != null)
        {
            closeButton.RegisterCallback<ClickEvent>(ev => OnCloseShopButtonClick());
        }
        
        //Find goldAmount Label in the shop
        goldAmount = root.Q<Label>("MoneyAmt");
        root.style.display = DisplayStyle.None;
    }
    
    private void OnCloseShopButtonClick()
    {
        audioSource.PlayOneShot(onCloseSound);
        //shop.SetActive(!shop.activeSelf);
        var root = shopUIDocument.rootVisualElement;
        root.style.display = DisplayStyle.None;
        //transform.localScale = Vector3.zero;
    }

    public void OnOpen()
    {
        var root = shopUIDocument.rootVisualElement;

        // Check if the game state is GS_BATTLE and abort opening the shop if true
        if (GameManager.instance.IsBattleState())
        {
            Debug.Log("Shop cannot be opened during battle.");
            return; // Prevents the shop from being opened during battle
        }

        //If already open, just close.
        if (root.style.display == DisplayStyle.Flex)
        {
            root.style.display = DisplayStyle.None;
            return;
        }

        //Show and re-render.
        root.style.display = DisplayStyle.Flex;
        PopulateShop();
    }

    public void CloseShop()
    {
        var root = shopUIDocument.rootVisualElement;
        root.style.display = DisplayStyle.None;
        audioSource.PlayOneShot(onCloseSound);
    }

    public void UpdateGoldAmount(int value)
    {
        if (goldAmount == null)
            Debug.Log("null");
        goldAmount.text = value.ToString();
    }

    void PopulateShop()
    {
        shopScrollView.Clear();
        int itemsPerRow = 2;
        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        shopScrollView.Add(row);

        for (int i = 0; i < currentItemCollection.items.Length; i++)
        {
            if (i % itemsPerRow == 0 && i != 0)
            {
                row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                shopScrollView.Add(row);
            }

            var item = currentItemCollection.items[i];
            var itemElement = itemTemplate.CloneTree();

            // Set item details
            var card = itemElement.Q<VisualElement>("Card");
            var buyButton = itemElement.Q<VisualElement>("BuyButton");
            var priceLabel = buyButton.Q<Label>("Price");

            // Assume item.icon is a Sprite
            if (item.icon != null)
            {
                // Create an Image element and set the sprite
                var image = new Image();
                image.image = item.icon.texture; // Set the texture from the sprite
                card.Add(image);
            }

            priceLabel.text = item.price.ToString();

            
            
            // Add click event listener to buyButton
            buyButton.RegisterCallback<ClickEvent>(evt => PurchaseItem(item));

            // Add the item element to the current row
            row.Add(itemElement);
            
            // Disable the priceLabel if not enough money
            if (!HasEnoughMoney(item.price))
            {
                buyButton.SetEnabled(false);
            }
            else
            {
                buyButton.SetEnabled(true);
            }
        }
    }

    private bool HasEnoughMoney(int price)
    {
        if (marketManager != null)
        {
            Debug.Log("Market Manager gold: " + marketManager.Gold + " price: " + price);
            return marketManager.Gold >= price;
        }
        return false;
    }

    void PurchaseItem(ShopItem item)
    {
        if (MarketManager.instance.Gold < item.price)
        {
            return;
        }
        Debug.Log("Purchased " + item.itemName + " for " + item.price);
        audioSource.PlayOneShot(onBuySound);
        
        
        
        // Add purchase logic here
        switch (item.itemName) {
            case "Cannon":
                this.cannonsBought += 1;
                this.cannonCard.SetActive(true);
                
                this.cannonCardText.text = cannonsBought.ToString();
                MarketManager.instance.spendGold(MarketManager.Items.CANNON);
                break;
            case "Dice":
                this.diceBought += 1;
                this.diceCard.SetActive(true);

                this.diceCardText.text = diceBought.ToString();
                MarketManager.instance.spendGold(MarketManager.Items.DICE);
                break;
            case "Coin":
                this.coinsBought += 1;
                this.coinCard.SetActive(true);

                this.coinCardText.text = coinsBought.ToString();
                MarketManager.instance.spendGold(MarketManager.Items.COIN);
                break;
        }
        //Re-render the shop.
        PopulateShop();

    }

    public void useCard(string name) {
        switch (name) {
            case "Cannon":
                this.cannonsBought -= 1;
                if (cannonsBought == 0)
                    this.cannonCard.SetActive(false);
                this.cannonCardText.text = cannonsBought.ToString();
                break;
            case "Dice":
                this.diceBought -= 1;
                if(diceBought == 0)
                    this.diceCard.SetActive(false);
                this.diceCardText.text = diceBought.ToString();
                break;
            case "Coin":
                this.coinsBought -= 1;
                if (coinsBought == 0)
                    this.coinCard.SetActive(false);
                this.coinCardText.text = coinsBought.ToString();
                break;
        }
    }
}
