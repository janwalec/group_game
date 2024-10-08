using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Chain {
    public int chainID;
    public LinkedList<MyTile> tileChain;
    public List<PooledChain> pooledObjectChain;

    public Chain(LinkedList<MyTile> _tileChain, List<PooledChain> _pooledObjectChain, int ID) {
        tileChain = _tileChain;
        pooledObjectChain = _pooledObjectChain;
        chainID = ID;

    }

    //modifiers

}

public class ChainControler : MonoBehaviour
{
    private ChainPool chainPool;
    private ChainGenerator chainGenerator;
        
    private List<Chain> myChains = new List<Chain>();

    private int currID = 0;

    public delegate void KeyPressAction();


    void Start()
    {
        chainPool = ChainPool.SharedInstance;
        chainGenerator = FindObjectOfType<ChainGenerator>();
        chainGenerator.OnChainComplete += UpdateObjectsOnChainComplete;
    }

    //be notified if Chain is added
    private void UpdateObjectsOnChainComplete(LinkedList<MyTile> completedChain) {
        Debug.Log("Chain completed with " + completedChain.Count + " tiles.");

        processAndSaveGivenChain(completedChain);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(myChains.Count > 0) {
                Chain tmp = myChains[myChains.Count - 1];
                deleteChain(tmp.chainID);
            }
            
        }

    }


    private void processAndSaveGivenChain(LinkedList<MyTile> completedChain) { // get completed Chain with its tiles
        int i = 0;
        int amount = completedChain.Count;
        
        // get (transparent) object to indicate where Chain is
        List<PooledChain> pooledObjects = chainPool.GetObjects(amount);
        
        // init random color for a Chain
        System.Random rnd = new System.Random();
        int r = rnd.Next(256), g = rnd.Next(256), b = rnd.Next(256);
        Color color = new Color(r / 255f, g / 255f, b / 255f, 1f);

        foreach (MyTile t in completedChain) {
            MyTile currTile = t;            

            // set random color
            SpriteRenderer spriteRenderer = pooledObjects[i].gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = color; 

            // transform position of game object and its canvas (text on object)
            Vector3 position = chainGenerator.getGenPosition(currTile);
            pooledObjects[i].gameObject.transform.position = position;
            pooledObjects[i].canvas.transform.position = position;

            i += 1;
        }

        // create new Chain, set the text to it and save
        Chain newChain = new Chain(completedChain, pooledObjects, currID++);
        setTextForChain(newChain);
        myChains.Add(newChain);

    }

    private void setTextForChain(Chain Chain) {
        int i = 0;
        foreach (PooledChain chainPart in Chain.pooledObjectChain) {
            if(i == 0) {
                chainPart.changeText("h" + myChains.Count);
            }
            else if(i == Chain.pooledObjectChain.Count - 1) {
                chainPart.changeText("t" + myChains.Count);
            } else {
                chainPart.changeText("-" + myChains.Count);
            }
            i++;
        }
    }

    private void deleteChain(int ID) {
        Chain toDelete = new Chain(null, null, -1);

        // search for a chain with given ID
        foreach(Chain ch in myChains) {
            if(ch.chainID == ID) {
                toDelete = ch;
                break;
            }
        }

        // no chain with this ID found
        if(toDelete.chainID == -1)
            return;

        int length = toDelete.pooledObjectChain.Count;
        int deletedCount = chainPool.SetChainInactive(toDelete.pooledObjectChain);
        myChains.Remove(toDelete);

    }


}