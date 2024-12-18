using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    public static MarketManager instance;

    public enum Items { CANNON, DICE, COIN, NONE }
    private int gold;
    public TMP_Text goldAmount;

    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private ShopManager shopManager;
    public int Gold
    {
        get => gold;
    }
    private const int GOLD_AT_BEGINNING = 15;
    private Dictionary<Items, int> prices = new Dictionary<Items, int>();
    public Dictionary<Items, int> Prices
    {
        get { return prices; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Optional: to keep the instance persistent across scenes
        }
    }
    void Start()
    {
        //sets the initial amount of gold and items' prices
        //instance = this;
        gold = GOLD_AT_BEGINNING;
        prices.Add(Items.DICE, 20);
        prices.Add(Items.CANNON, 30);
        prices.Add(Items.COIN, 10);
        goldAmount.text = gold.ToString();
        gameUIController.UpdateGoldAmount(Gold);
    }
    public void spendGold(Items item)
    {
        gold -= prices[item];
        gameUIController.UpdateGoldAmount(Gold);
        shopManager.UpdateGoldAmount(Gold);
        //goldAmount.text = gold.ToString();
    }

    public void sellItemForGold(Items item, float modifier) {
        
        gold += (int)(modifier *prices[item]);
        gameUIController.UpdateGoldAmount(Gold);
        shopManager.UpdateGoldAmount(Gold);
    }

    public void earnGold(int amount)
    {
        gold += amount;
        gameUIController.UpdateGoldAmount(Gold);
        shopManager.UpdateGoldAmount(Gold);
        //goldAmount.text = gold.ToString();
        //goldAmount.text = gold.ToString();
    }

    public bool canAfford(Items item)
    {
        if(prices.ContainsKey(item))
        {
            if (prices[item] <= gold)
            {
                return true;
            }
        }
        return false;
    }
    
    void Update()
    {
        // Check if the 'g' key is pressed
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Call the earnGold function with 1000 as the argument
            earnGold(1000);
        }
    }

}