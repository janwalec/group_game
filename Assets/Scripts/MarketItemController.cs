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
            frameImage.color = Color.red;
        }
    }

    public void unselectObject()
    {
        frameImage.color = Color.white;
    }

    public void putObject(Vector3 position)
    {
        if (objectType == MarketManager.Items.CANNON)
        {
            GameObject new_object = CannonPool.SharedInstance.GetPooledObject();
            if (new_object != null)
            {
                new_object.transform.position = position;

                new_object.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("OnLand");
                new_object.SetActive(true);
            }
        }
        MarketManager.instance.spendGold(objectType);

    }
}
