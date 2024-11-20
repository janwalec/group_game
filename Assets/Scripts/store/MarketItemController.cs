using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;



public class MarketItemController : MonoBehaviour
{
    
    [SerializeField]
    Image frameImage;
    public MarketManager.Items objectType;

    bool selected = false;

    protected AudioSource audioSource;
    [SerializeField] protected AudioClip onSelectSound;

    [SerializeField] protected AudioClip onSellSound;

    private ShopManager shopManager;
    void Start()
    {
        this.shopManager = GameManager.instance.getShopManager();
    }

    void Update()
    {
        
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();


    }
    public MarketManager.Items getObjectType()
    {
        return objectType;
    }

    
    public void SelectObject()
    {
        if (!selected)
        {
            selected = true;
            if(audioSource != null)
                audioSource.PlayOneShot(onSelectSound, audioSource.volume);
            //informs the tilemap what kind of object is currently selected 
            //and chenges its color to inform the user
            if (MarketManager.instance.canAfford(objectType))
            {
                GameManager.instance.getTilemap().selectObject(this);
                if(frameImage != null)
                    frameImage.color = new Color(frameImage.color.r - 0.1f, frameImage.color.g, frameImage.color.b - 0.1f);
            }
        }
    }

    public void Sell() {
        float modifier = 0.7f;

        //TODO
        audioSource.PlayOneShot(onSellSound, audioSource.volume);
        
        if (objectType == MarketManager.Items.CANNON) {
            shopManager.useCard("Cannon");
        }
        if (objectType == MarketManager.Items.COIN) {
            shopManager.useCard("Coin");
        }
        if (objectType == MarketManager.Items.DICE) {
            shopManager.useCard("Dice");
        }
        MarketManager.instance.sellItemForGold(objectType, modifier);
   
        

    }

    public void unselectObject()
    {

        if (frameImage != null)
        {
            frameImage.color = Color.white;
        }
        selected = false;
    }

    public GameObject putObject(Vector3 position)
    {
        //pools the object and sets its position
        GameObject new_object = null;
        if (objectType == MarketManager.Items.CANNON)
        {
            new_object = ItemPool.SharedInstance.GetPooledCannon();
            new_object.transform.position = position;
            shopManager.useCard("Cannon");
        }
        else if (objectType == MarketManager.Items.DICE)
        {
            new_object = ItemPool.SharedInstance.GetPooledDice();
            new_object.transform.position = new Vector3(position.x-0.1f, position.y-0.05f, position.z);
            shopManager.useCard("Dice");
        }
        else if (objectType == MarketManager.Items.COIN)
        {
            new_object = ItemPool.SharedInstance.GetPooledCoin();
            new_object.transform.position = new Vector3(position.x-0.1f, position.y-0.05f, position.z);
            shopManager.useCard("Coin");
        }
        if (new_object != null)
        {

            new_object.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("OnLand");
            new_object.SetActive(true);
        }
        //MarketManager.instance.spendGold(objectType);
        
        

        return new_object;
    }

    public Vector3 adjustPosition(Vector3 position)
    {
       
        if (objectType == MarketManager.Items.DICE)
        {
            return new Vector3(position.x - 0.1f, position.y - 0.05f, position.z);
        }
        else if (objectType == MarketManager.Items.COIN)
        {
            return new Vector3(position.x - 0.1f, position.y - 0.05f, position.z);
        }
        return position;
    }
}
