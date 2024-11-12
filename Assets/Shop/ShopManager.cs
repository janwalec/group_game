using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopManager : MonoBehaviour
{
    public ItemCollection currentItemCollection;
    public VisualTreeAsset itemTemplate;
    private ScrollView shopScrollView;
    public UIDocument shopUIDocument; // Reference to the overall shop UI document
    private Button closeButton;
    
    public GameObject shop;

    private MarketItemController marketItemController;
    Label goldAmount;
    void OnEnable()
    {
        this.marketItemController = GameManager.instance.getMarketController();


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
        goldAmount = root.Q<Label>("GoldAmountLabel");
    }
    
    private void OnCloseShopButtonClick()
    {
        shop.SetActive(!shop.activeSelf);
    }
    
    public void UpdateGoldAmount(int value)
    {
        goldAmount.text = value.ToString();
    }

    void PopulateShop()
    {
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
        }
    }

    void PurchaseItem(ShopItem item)
    {
        Debug.Log("Purchased " + item.itemName + " for " + item.price);
        // Add purchase logic here
        Debug.Log(this.marketItemController);
    }
}
