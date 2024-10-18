using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 



public struct PooledChain {
    public GameObject gameObject;
    public Canvas canvas;

    public void changeText(string newText) {
        TextMeshProUGUI textComponent = canvas.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = newText;
    }
}
public class ChainPool : MonoBehaviour
{

    public static ChainPool SharedInstance;
    public List<PooledChain> pooledObjects; // all pulled objects to be used
    public GameObject objectToPool;
    public int amountToPool;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledObjects = new List<PooledChain>();
        GameObject tmpGameObject;
        Canvas tmpCanvas;
        for (int i = 0; i < amountToPool; i++)
        {
            tmpGameObject = Instantiate(objectToPool);
            tmpGameObject.SetActive(false);
            tmpCanvas = tmpGameObject.GetComponentInChildren<Canvas>();
            
            PooledChain tmpPooledChain = new PooledChain();
            
            tmpPooledChain.gameObject = tmpGameObject;
            tmpPooledChain.canvas = tmpCanvas;

            pooledObjects.Add(tmpPooledChain);
        }
    }

    public List<PooledChain> GetObjects(int n)
    {
        List<PooledChain> tmp = new List<PooledChain>();
        tmp.Clear();
        for (int i = 0; i < amountToPool; i++)
        {
            if(tmp.Count == n)
                break;
                
            if (!pooledObjects[i].gameObject.activeInHierarchy)
            {
                tmp.Add(pooledObjects[i]);
                pooledObjects[i].gameObject.SetActive(true);
                pooledObjects[i].gameObject.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("OnLand");
                pooledObjects[i].canvas.sortingLayerID = SortingLayer.NameToID("OnLand");                
            }
            
        }
        if(tmp.Count == 0)
            return null;
        return tmp;
    }

    public int SetChainInactive(List<PooledChain> toDelete) {
        int howManyCleared = 0;
        foreach(PooledChain ch in toDelete) {
            ch.gameObject.SetActive(false);
            howManyCleared++;
        }
        return howManyCleared;
    }

    void Update()
    {
        
    }

}