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
    public int Gold
    {
        get => gold;
    }
    private const int GOLD_AT_BEGINNING = 100;
    private Dictionary<Items, int> prices = new Dictionary<Items, int>();
    public Dictionary<Items, int> Prices
    {
        get { return prices; }
    }

    void Start()
    {
        //sets the initial amount of gold and items' prices
        instance = this;
        gold = GOLD_AT_BEGINNING;
        prices.Add(Items.DICE, 20);
        prices.Add(Items.CANNON, 30);
        prices.Add(Items.COIN, 10);
        goldAmount.text = gold.ToString();
        gameUIController.UpdateGoldAmount(Gold);
    }
    void Update()
    {

    }
    public void spendGold(Items item)
    {
        gold -= prices[item];
        gameUIController.UpdateGoldAmount(Gold);
        //goldAmount.text = gold.ToString();
    }
    public void earnGold(int amount)
    {
        gold += amount;
        gameUIController.UpdateGoldAmount(Gold);
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

}