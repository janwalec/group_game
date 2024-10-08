using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;



public class ChainGenerator : MonoBehaviour
{
    public static ChainGenerator SharedInstance;

    public event Action< LinkedList<MyTile> > OnChainComplete; //to notify ChainControler

    public bool isLeftMouseButtonPressed = false;
    public bool isLeftMouseButtonReleased = true;

    

    
    
    public TIleMapGenerator tilemap;
    private LinkedList<MyTile> tempList = new LinkedList<MyTile>();
    
    private List<LinkedList<MyTile>> listOfChains = new List<LinkedList<MyTile>>();

    private MyTile lastTile;

    private void Start() {
        tilemap = GameManager.instance.getTilemap();
        
        SharedInstance = this;
    }

    void Update() {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right pressed");
            isLeftMouseButtonPressed = true;
            isLeftMouseButtonReleased = false;  
        }
        
        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("Right released");
            isLeftMouseButtonPressed = false;
            isLeftMouseButtonReleased = true;
            
        }
        createTempChain(); 
    }

    private void NotifyChainComplete() {
        OnChainComplete?.Invoke(tempList);
    }

    private void createTempChain() {
        //clear temp list if not dragging
        if(tempList.Count != 0 &&!isLeftMouseButtonPressed && isLeftMouseButtonReleased) {
            listOfChains.Add(tempList);

            NotifyChainComplete();
            Debug.Log("cleared");
            tempList.Clear();
        }

        //dragging mouse over board
        if(isLeftMouseButtonPressed && !isLeftMouseButtonReleased) {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //get position
            MyTile newTile = tilemap.getTileFromMousePosition(mouseWorldPos);   //get tile from this position
            if(checkConditionForTile(newTile)) {
                tempList.AddLast(newTile);
                Debug.Log("Added " + tempList.Count);
            }
            
        }   
    }
    
    private bool checkConditionForTile(MyTile checkTile) {
        
        if(tempList.Contains(checkTile)) //already in the list
            return false;
        
        if(!tilemap.checkIfLand(checkTile))
            return false;

        if(tempList.Count > 0)
            if(!tilemap.checkIfNeighbours(tempList.Last.Value, checkTile))
                return false;

        return true;
    }

    public Vector3 getGenPosition(MyTile tile) {
        return tilemap.tm.CellToWorld(tile.tilePosition);
    }

}