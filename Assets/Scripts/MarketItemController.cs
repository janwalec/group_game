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


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public MarketManager.Items getObjectType()
    {
        return objectType;
    }

    
    public void SelectObject()
    {
        if (MarketManager.instance.canAfford(objectType))
        {
            GameManager.instance.getTilemap().selectObject(this);
            frameImage.color = new Color(frameImage.color.r-0.1f, frameImage.color.g, frameImage.color.b-0.1f);
        }
    }

    public void unselectObject()
    {
        frameImage.color = Color.white;
    }

    public void putObject(Vector3 position)
    {
        GameObject new_object = null;
        if (objectType == MarketManager.Items.CANNON)
        {
            new_object = CannonPool.SharedInstance.GetPooledObject();
            new_object.transform.position = position;
        }
        else if (objectType == MarketManager.Items.DICE)
        {
            new_object = DicePool.SharedInstance.GetPooledObject();
            new_object.transform.position = new Vector3(position.x-0.1f, position.y-0.05f, position.z);
        }
        else if (objectType == MarketManager.Items.COIN)
        {
            new_object = CoinPool.SharedInstance.GetPooledObject();
            new_object.transform.position = new Vector3(position.x-0.1f, position.y-0.05f, position.z);
        }
        if (new_object != null)
        {
            //new_object.transform.position = position;

            new_object.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("OnLand");
            new_object.SetActive(true);
        }
        MarketManager.instance.spendGold(objectType);
    }
}
