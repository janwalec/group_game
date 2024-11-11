using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UIElements;
using UnityEditor.Tilemaps;
using Unity.VisualScripting;
using System.ComponentModel;

public class MyTile {
    public enum TileType { WATER, LAND, ROCK, CANNON, DICE, COIN}

    public int y, x;
    public Vector3Int tilePosition;
    public TileBase tile;
    public bool occupied;
    
    public bool usedForChain;
   
    public MyTile[] neighbours;

    public TileType tileType;
    public ModifierController modifier;
    public CannonController cannon;
 
    public MyTile(int y, int x, TileBase tile, TileType tileType) {
        this.y = y;
        this.x = x;
        this.tilePosition = new Vector3Int(x, y, 0);
        this.tile = tile;
        this.tileType = tileType;
        this.neighbours  = new MyTile[6];
        this.occupied = false;
        this.usedForChain = false;
        this.modifier = null;
        this.cannon = null;
    }

    public static bool operator ==(MyTile t1, MyTile t2) {
        return t1.x == t2.x && t1.y == t2.y && t1.tile == t2.tile;
    }

    public static bool operator !=(MyTile t1, MyTile t2) {
        return !(t1 == t2);
    }

    public override bool Equals(object obj) {
        if (!(obj is MyTile)) {
            return false;
        }
        MyTile other = (MyTile)obj;
        return this == other;
    }

    public override int GetHashCode() {
        return (x, y, tile).GetHashCode();
    }

    public MyTile GetNeighbourAt(int idx)
    {
        return neighbours[idx];
    }

}


public class TIleMapGenerator : MonoBehaviour
{
    public Tilemap tm;
    public Tile newTile;
    public Tile landTile;
    public TileBase greyTile;

    public TileBase rockTile;

    public GameObject tileCollider;

    private MyTile[,] tilesArray;
    private int size_x = 0, size_y = 0;
    private int y_min, x_min, y_max, x_max;
    private AudioSource audioSource;
    [SerializeField] public AudioClip onPlacingItemSound;

    //private GameManager.Items itemSelected = GameManager.Items.NONE;
    private MarketItemController selector;
    // Start is called before the first frame update

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        tm = GetComponent<Tilemap>();
        
        BoundsInt bounds = tm.cellBounds;
        this.size_x = bounds.size.x; 
        this.size_y = bounds.size.y;
        this.y_min = bounds.yMin;
        this.y_max = bounds.yMax;
        this.x_min = bounds.xMin;
        this.x_max = bounds.xMax;

        tilesArray = new MyTile[size_y, size_x];

        for (int i = bounds.yMin; i < bounds.yMax; i++) {
            for (int j = bounds.xMin; j < bounds.xMax; j++) {
                Vector3Int tilePosition = new Vector3Int(j, i, 0);
                TileBase tile = tm.GetTile(tilePosition);
                if (tile == landTile)
                {
                    tilesArray[i - bounds.yMin, j - bounds.xMin] = new MyTile(i, j, tile, MyTile.TileType.LAND);
                    
                    Vector3 worldPos = getWorldPosition(tilePosition);
                    Debug.Log("Tile position: " + tilePosition + ", World position: " + worldPos);
                    GameObject colliderObject = Instantiate(tileCollider, worldPos, Quaternion.identity);
                    colliderObject.SetActive(true); 
                }
                else if (tile == rockTile) {
                    tilesArray[i - bounds.yMin, j - bounds.xMin] = new MyTile(i, j, tile, MyTile.TileType.ROCK);
                    Vector3 worldPos = getWorldPosition(tilePosition);
                    Debug.Log("Tile position: " + tilePosition + ", World position: " + worldPos);
                    GameObject colliderObject = Instantiate(tileCollider, worldPos, Quaternion.identity);
                    colliderObject.SetActive(true); 
                }
                else
                {
                    tilesArray[i - bounds.yMin, j - bounds.xMin] = new MyTile(i, j, tile, MyTile.TileType.WATER);
                }
            }
        }   
        CreateTileNeighbours();
        GameManager.instance.setTilemap(this);
    }

    //finds the neighbours of a tile
    void CreateTileNeighbours() {
        for (int i = y_min; i < y_max; i++) {
            for (int j = x_min; j < x_max; j++) {
                int l = i - y_min;
                int k = j - x_min;
                MyTile tile = tilesArray[l, k];

                // Even row (l % 2 == 0)
                if (l % 2 != 0) {
                    if (l + 1 < size_y)
                        tile.neighbours[0] = tilesArray[l + 1, k]; //top left
                    
                    if (l + 1 < size_y && k + 1 < size_x)
                        tile.neighbours[1] = tilesArray[l + 1, k + 1]; //top right
                    
                    if (k - 1 >= 0)
                        tile.neighbours[2] = tilesArray[l, k - 1]; //middle left
                    
                    if (k + 1 < size_x)
                        tile.neighbours[3] = tilesArray[l, k + 1]; //middle right
                    
                    if (l - 1 >= 0)
                        tile.neighbours[4] = tilesArray[l - 1, k]; //bottom left
                    
                    if (l - 1 >= 0 && k + 1 < size_x)
                        tile.neighbours[5] = tilesArray[l - 1, k + 1]; //bottom right
                } 
                // Odd row (l % 2 != 0)
                else {
                    if (l + 1 < size_y && k - 1 >= 0)
                        tile.neighbours[0] = tilesArray[l + 1, k - 1]; //top left
                    
                    if (l + 1 < size_y)
                        tile.neighbours[1] = tilesArray[l + 1, k]; //top right
                    
                    if (k - 1 >= 0)
                        tile.neighbours[2] = tilesArray[l, k - 1]; //middle left
                    
                    if (k + 1 < size_x)
                        tile.neighbours[3] = tilesArray[l, k + 1];// middle right
                    
                    if (l - 1 >= 0 && k - 1 >= 0)
                        tile.neighbours[4] = tilesArray[l - 1, k - 1]; //bottom left
                    
                    if (l - 1 >= 0)
                        tile.neighbours[5] = tilesArray[l - 1, k]; //bottom right
                }
            }
        }
    }

    //changes tile to another one
    void ChangeTiles(Vector3Int position, TileBase drawThis)
    {
        int x_search = position.x - x_min;
        int y_search = position.y - y_min;
        MyTile tile = tilesArray[y_search, x_search];

            if(tile.tile != null) {
                tm.SetTile(tile.tilePosition, drawThis);
                
                TilemapRenderer renderer = tm.GetComponent<TilemapRenderer>();
               
            }
    }

    //displays all neighbours of a tile
    void DisplayNeighbours(Vector3Int position) {
        int x_search = position.x - x_min;  
        int y_search = position.y - y_min;
        MyTile tile = tilesArray[y_search, x_search];
        
        if(tile.tile != null) {
            for (int i = 0; i < 6; i++) {
                if(tile.neighbours[i].tile != null) {
                    tm.SetTile(tile.neighbours[i].tilePosition, newTile);
                }
            }
        }
    }
    

    //checks if given position is a part of a tilemap
    private bool checkIfTilemap(Vector3Int gridPosition)
    {
        
        if (gridPosition.y - tm.cellBounds.yMin >= 0  && gridPosition.y - tm.cellBounds.yMin < tm.cellBounds.size.y
            && gridPosition.x - tm.cellBounds.xMin >= 0 && gridPosition.x - tm.cellBounds.xMin < tm.cellBounds.size.x) {
            return true;
        }
        return false;
    }

    //sets current selector
    public void selectObject(MarketItemController selector_)
    {
        selector = selector_;

    }

    public Vector3 getWorldPosition(Vector3Int gridPosition)
    {
        return tm.CellToWorld(gridPosition);
    }

    void OnMouseDown()
    {
        if(selector == null)
        {
            return;
        }

        //after clicking anywhere on the screen, the selector (market item) is no longer chosen
        

        PlaceAnItem();
      
            
    }

    public bool PlaceAnItem(GameObject newObject = null)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = tm.WorldToCell(mouseWorldPos);

        int x_search = gridPosition.x - x_min;
        int y_search = gridPosition.y - y_min;
        MyTile tile = tilesArray[y_search, x_search];


        //cannot place an item on an occupied tile
        if (tile.occupied)
        {
            selector.unselectObject();
            selector = null;
            return false;
        }

        Vector3 placingPosition = tm.CellToWorld(gridPosition);

        placingPosition.x += 0.1f;
        placingPosition.y += 0.1f;
        placingPosition.z = 0.0f;

        //places an item on specific location
        if (checkIfTilemap(gridPosition))
        {
            if (tilesArray[y_search, x_search].tileType == MyTile.TileType.LAND)
            {
                audioSource.PlayOneShot(onPlacingItemSound, audioSource.volume);

                if(newObject == null)
                    newObject = selector.putObject(placingPosition);
                else
                {
                    newObject.transform.position = selector.adjustPosition(placingPosition);
                }
                //tm.SetTile(tile.tilePosition, greyTile);
                tilesArray[y_search, x_search].occupied = true;
                if (selector.objectType == MarketManager.Items.COIN)
                {
                    tilesArray[y_search, x_search].tileType = MyTile.TileType.COIN;
                    tilesArray[y_search, x_search].modifier = newObject.GetComponent<CoinController>();
                }
                if (selector.objectType == MarketManager.Items.DICE)
                {
                    tilesArray[y_search, x_search].tileType = MyTile.TileType.DICE;
                    tilesArray[y_search, x_search].modifier = newObject.GetComponent<DiceController>();
                }
                if (selector.objectType == MarketManager.Items.CANNON)
                {
                    tilesArray[y_search, x_search].tileType = MyTile.TileType.CANNON;
                    tilesArray[y_search, x_search].cannon = newObject.GetComponentInChildren<CannonController>();
                    Debug.Log(tilesArray[y_search, x_search].cannon == null);
                }
                selector.unselectObject();
                selector = null;
                return true;
            }
        }
        selector.unselectObject();
        selector = null;
        return false;
    }


    //marks the tile as occupied so that nothong else can be places there
    public void occupyTile(Vector3 position)
    {
        Vector3Int gridPosition = tm.WorldToCell(position);

        int x_search = gridPosition.x - x_min;
        int y_search = gridPosition.y - y_min;
        tilesArray[y_search, x_search].occupied = true;

    }

    // Update is called once per frame
    void Update()
    {
        /*
       
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("click");
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = tm.WorldToCell(mouseWorldPos);
            /*gridPosition.z = 0;
            int xx = gridPosition.x;
            int yy = gridPosition.y;
            Debug.Log("y: " + yy.ToString() + "  x: " + xx.ToString());*/
        /*
            if(!checkIfTilemap(gridPosition))
            {
                return;
            }
            int x_search = gridPosition.x - x_min;
            int y_search = gridPosition.y - y_min;
            
            if (tilesArray[y_search, x_search].tile == landTile)
            {
                DisplayNeighbours(gridPosition);
            }
            //ChangeTiles(gridPosition);
        }
            */
    }

    public MyTile getTileFromMousePosition(Vector3 mouseWorldPos) {
        Vector3Int gridPosition = tm.WorldToCell(mouseWorldPos);
        int x_search = gridPosition.x - x_min;
        int y_search = gridPosition.y - y_min;
        return tilesArray[y_search, x_search];
    }

    public void releaseTile(MyTile tile)
    {
        tile.occupied = false;
        tile.usedForChain = false;
        tile.tileType = MyTile.TileType.LAND;
        tile.cannon = null;
        tile.modifier = null;

    }

    public bool checkIfLand(MyTile t) {
        if(t.tileType == MyTile.TileType.LAND)
            return true;
        return false;
    }

    public bool checkIfModifier(MyTile t)
    {
        if (t.tileType == MyTile.TileType.COIN || t.tileType == MyTile.TileType.DICE)
            return true;
        return false;
    }

    public bool checkIfCannon(MyTile t)
    {
        if (t.tileType == MyTile.TileType.CANNON)
            return true;
        return false;
    }

    public bool checkIfNeighbours(MyTile tile, MyTile potentialNeighbour) {
        foreach (MyTile neighbour in tile.neighbours) {
            if(neighbour == potentialNeighbour)
                return true;
        }
        return false;
    }

    
    //change that from many tiles (0, 1 ,2)
    public void drawChain(TileBase head, TileBase tail, TileBase node, LinkedList<MyTile> chain) {

        foreach(MyTile t in chain) {
            ChangeTiles(t.tilePosition, node);
        }

        ChangeTiles(chain.First.Value.tilePosition, head);
        ChangeTiles(chain.Last.Value.tilePosition, tail);
    }

    public void setUsedForChain(int y, int x, bool used) {
        this.tilesArray[y - y_min, x - x_min].usedForChain = used;
    }

    



}
