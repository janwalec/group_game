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

    protected AudioSource audioSource;
    [SerializeField] protected AudioClip onSelectSound;
    void Start()
    {
        
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
        audioSource.PlayOneShot(onSelectSound, audioSource.volume); 
        //informs the tilemap what kind of object is currently selected 
        //and chenges its color to inform the user
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

    public GameObject putObject(Vector3 position)
    {
        //pools the object and sets its position
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

            new_object.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("OnLand");
            new_object.SetActive(true);
        }
        MarketManager.instance.spendGold(objectType);
        return new_object;
    }
}
