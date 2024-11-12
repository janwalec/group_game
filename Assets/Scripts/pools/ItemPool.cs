using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
   
    

    public static ItemPool SharedInstance;
    public List<GameObject> pooledDice = new List<GameObject>();
    public List<GameObject> pooledCoin = new List<GameObject>();
    public List<GameObject> pooledCannon = new List<GameObject>();
    public List<GameObject> pooledCannonBall = new List<GameObject>();
    public List<GameObject> pooledLineRenderer = new List<GameObject>();

    public List<GameObject> pooledCannonCards = new List<GameObject>();
    public List<GameObject> pooledCoinCards = new List<GameObject>();
    public List<GameObject>  pooledDiceCards = new List<GameObject>();
    
    public GameObject coinToPool;
    public GameObject diceToPool;
    public GameObject cannonToPool;
    public GameObject cannonBallToPool;
    public GameObject lineRendererToPool;

    public GameObject cannonCard;
    public GameObject coinCard;
    public GameObject diceCard;

    public int coinCardAmountToPool;
    public int diceCardAmountToPool;
    public int cannonCardAmountToPool;

    public int coinAmountToPool;
    public int diceAmountToPool;
    public int cannonAmountToPool;
    public int cannonBallAmountToPool;
    public int lineRenderAmountToPool;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        //Debug.Log("a" + pooledCoin.Count);
        InitializePooling(pooledCoin, coinAmountToPool, coinToPool);
        //Debug.Log("b" + pooledCoin.Count);
        InitializePooling(pooledDice, diceAmountToPool, diceToPool);
        InitializePooling(pooledCannon, cannonAmountToPool, cannonToPool);
        InitializePooling(pooledCannonBall, cannonBallAmountToPool, cannonBallToPool);
        InitializePooling(pooledLineRenderer, lineRenderAmountToPool, lineRendererToPool);
        
        InitializePooling(pooledCannonCards, cannonCardAmountToPool, cannonCard);
        InitializePooling(pooledCoinCards, coinCardAmountToPool, coinCard);
        InitializePooling(pooledDiceCards, diceCardAmountToPool, diceCard);
    }

    private void InitializePooling(List<GameObject> pooledObjects, int amountToPool, GameObject objectToPool)
    {
        //pooledObjects = new List<GameObject>();
        GameObject tmp;
        //Debug.Log(pooledObjects.GetHashCode() + "a1");
        //Debug.Log(this.pooledCoin.GetHashCode() + "a2s");
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }
    public GameObject GetPooledDice()
    {
        return GetPooledObject(pooledDice, diceAmountToPool);
    }

    public GameObject GetPooledCoin()
    {
        return GetPooledObject(pooledCoin, coinAmountToPool);
    }

    public GameObject GetPooledCannon()
    {
        return GetPooledObject(pooledCannon, cannonAmountToPool);
    }
    public GameObject GetPooledCannonBall()
    {
        return GetPooledObject(pooledCannonBall, cannonBallAmountToPool);
    }
    public GameObject GetPooledLineRenderer()
    {
        return GetPooledObject(pooledLineRenderer, lineRenderAmountToPool);
    }

    public GameObject GetPooledCannonCard() {
        return GetPooledObject(pooledCannonCards, cannonCardAmountToPool);
    }

    public GameObject GetPooledDiceCard() {
        return GetPooledObject(pooledDiceCards, diceCardAmountToPool);
    }

    public GameObject GetPooledCoinCard() {
        return GetPooledObject(pooledCoinCards, coinCardAmountToPool);
    }

    

    public GameObject GetPooledObject(List<GameObject> pooledObjects, int amountToPool)
    {
        //allows for pooling dice modifiers
        //Debug.Log(pooledObjects.Count);
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
