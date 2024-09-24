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

    public int Gold
    {
        get => gold;
    }
    private const int GOLD_AT_BEGINNING = 1000;
    private Dictionary<Items, int> prices = new Dictionary<Items, int>();
    public Dictionary<Items, int> Prices
    {
        get { return prices; }
    }

    void Start()
    {
        instance = this;
        gold = GOLD_AT_BEGINNING;
        prices.Add(Items.DICE, 20);
        prices.Add(Items.CANNON, 20);
        prices.Add(Items.COIN, 20);
        goldAmount.text = gold.ToString();
    }
    void Update()
    {

    }
    public void spendGold(Items item)
    {
        gold -= prices[item];
        goldAmount.text = gold.ToString();
    }
    public void earnGold(int amount)
    {
        gold += amount;
        goldAmount.text = gold.ToString();
        goldAmount.text = gold.ToString();
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