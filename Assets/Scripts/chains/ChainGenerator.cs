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

    private AudioSource audioSource;
    [SerializeField] private AudioClip onChainCreatedSound;
    [SerializeField] private AudioClip onTileAddedSound;

    [SerializeField] LineRendererController currentlyCreated;
    private List<Vector3> currentPoints = new List<Vector3>();

    public enum Direction { TOP_LEFT, TOP_RIGHT, MIDDLE_LEFT, MIDDLE_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT }
    public enum Operation { MULTIPLICATION, ADDITION}
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

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void NotifyChainComplete() {
        OnChainComplete?.Invoke(tempList);
    }

    //clear temp list if not dragging
    public void playerStoppedDragging() {
        currentPoints.Clear();
        currentlyCreated.SetUpLine(currentPoints);
        audioSource.PlayOneShot(onChainCreatedSound);
        listOfChains.Add(tempList);
        AddOperationSigns();
        NotifyChainComplete();
        //Debug.Log("cleared");
        
        tempList.Clear();
    }

    private void AddOperationSigns()
    {
        LinkedListNode<MyTile> curr = tempList.Last;
        Direction direction = Direction.TOP_LEFT;
        Operation operation = Operation.ADDITION;
        while(curr.Previous != null)
        {
            if(curr.Previous.Value.tileType == MyTile.TileType.CANNON)
                break;
          
            if (curr.Value.tileType == MyTile.TileType.DICE && curr.Previous.Value.tileType == MyTile.TileType.DICE)
                operation = Operation.ADDITION;
            else
                operation = Operation.MULTIPLICATION;
            

            if(curr.Value.tileType == MyTile.TileType.DICE || curr.Value.tileType == MyTile.TileType.COIN)
            {

                if (curr.Previous.Value == curr.Value.GetNeighbourAt(0))
                    direction = Direction.TOP_LEFT;
                else if (curr.Previous.Value == curr.Value.GetNeighbourAt(1))
                    direction = Direction.TOP_RIGHT;
                else if (curr.Previous.Value == curr.Value.GetNeighbourAt(2))
                    direction = Direction.MIDDLE_LEFT;
                else if (curr.Previous.Value == curr.Value.GetNeighbourAt(3))
                    direction = Direction.MIDDLE_RIGHT;
                else if (curr.Previous.Value == curr.Value.GetNeighbourAt(4))
                    direction = Direction.BOTTOM_LEFT;
                else
                    direction = Direction.BOTTOM_RIGHT;
            }
            curr.Value.modifier.SetOperation(direction, operation);

            curr = curr.Previous;
        }
    }

   



    //dragging mouse over board
    public void playerIsDragging() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //get position
        MyTile newTile = tilemap.getTileFromMousePosition(mouseWorldPos);   //get tile from this position
        
        if(checkConditionForTile(newTile)) {
            currentPoints.Add(GameManager.instance.getTilemap().getWorldPosition(newTile.tilePosition));
            currentlyCreated.SetUpLine(currentPoints);
            tempList.AddLast(newTile);
            audioSource.PlayOneShot(onTileAddedSound, audioSource.volume);
            Debug.Log("Added " + tempList.Count);
        }
    }

    private void createTempChain() {
        if(tempList.Count != 0 &&!isLeftMouseButtonPressed && isLeftMouseButtonReleased) {
            playerStoppedDragging();
        }

        
        if(isLeftMouseButtonPressed && !isLeftMouseButtonReleased) {
            playerIsDragging();
        }   
    }
    
    private bool checkConditionForTile(MyTile checkTile) {
        
        if(tempList.Count == 0 && !tilemap.checkIfCannon(checkTile))
            return false;

        if(tempList.Contains(checkTile)) // already in the list
            return false;
        
        if(!tilemap.checkIfModifier(checkTile) && !tilemap.checkIfCannon(checkTile)) // it is not the modifier
            return false;

        if(tempList.Count > 0)
            if(!tilemap.checkIfNeighbours(tempList.Last.Value, checkTile))
                return false;
        
        return true;
    }

    public Vector3 getGenPosition(MyTile tile) {
        return tilemap.tm.CellToWorld(tile.tilePosition);
    }

    public void markUsedForChainTiles(LinkedList<MyTile> chain, bool used) {
        foreach(MyTile t in chain) {
            tilemap.setUsedForChain(t.y, t.x, used);
        }
    }
   
    
}