using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UIElements;
using UnityEditor.Tilemaps;

public struct MyTile {
    public int y, x;
    public Vector3Int tilePosition;
    public TileBase tile;
    public bool occupied;
   
    public MyTile[] neighbours;

    public MyTile(int y, int x, TileBase tile) {
        this.y = y;
        this.x = x;
        this.tilePosition = new Vector3Int(x, y, 0);
        this.tile = tile;
        this.neighbours  = new MyTile[6];
        this.occupied = false;
    }

}


public class TIleMapGenerator : MonoBehaviour
{
    public Tilemap tm;
    public Tile newTile;
    public Tile landTile;
    public TileBase greyTile;


    private MyTile[,] tilesArray;
    private int size_x = 0, size_y = 0;
    private int y_min, x_min, y_max, x_max;

    //private GameManager.Items itemSelected = GameManager.Items.NONE;
    private MarketItemController selector = null;
    // Start is called before the first frame update

    void Awake() {
        
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

                tilesArray[i - bounds.yMin, j - bounds.xMin] = new MyTile(i, j, tile);
            }
        }   
        CreateTileNeighbours();
        GameManager.instance.setTilemap(this);
    }

    void CreateTileNeighbours() {
        for (int i = y_min; i < y_max; i++) {
            for (int j = x_min; j < x_max; j++) {
                int l = i - y_min;
                int k = j - x_min;
                MyTile tile = tilesArray[l, k];

                // Even row (l % 2 == 0)
                if (l % 2 == 0) {
                    if (l + 1 < size_y)
                        tile.neighbours[0] = tilesArray[l + 1, k];
                    
                    if (l + 1 < size_y && k + 1 < size_x)
                        tile.neighbours[1] = tilesArray[l + 1, k + 1];
                    
                    if (k - 1 >= 0)
                        tile.neighbours[2] = tilesArray[l, k - 1];
                    
                    if (k + 1 < size_x)
                        tile.neighbours[3] = tilesArray[l, k + 1];
                    
                    if (l - 1 >= 0)
                        tile.neighbours[4] = tilesArray[l - 1, k];
                    
                    if (l - 1 >= 0 && k + 1 < size_x)
                        tile.neighbours[5] = tilesArray[l - 1, k + 1];
                } 
                // Odd row (l % 2 != 0)
                else {
                    if (l + 1 < size_y && k - 1 >= 0)
                        tile.neighbours[0] = tilesArray[l + 1, k - 1];
                    
                    if (l + 1 < size_y)
                        tile.neighbours[1] = tilesArray[l + 1, k];
                    
                    if (k - 1 >= 0)
                        tile.neighbours[2] = tilesArray[l, k - 1];
                    
                    if (k + 1 < size_x)
                        tile.neighbours[3] = tilesArray[l, k + 1];
                    
                    if (l - 1 >= 0 && k - 1 >= 0)
                        tile.neighbours[4] = tilesArray[l - 1, k - 1];
                    
                    if (l - 1 >= 0)
                        tile.neighbours[5] = tilesArray[l - 1, k];
                }
            }
        }
    }

    void ChangeTiles(Vector3Int position)
    {
        int x_search = position.x - x_min;
        int y_search = position.y - y_min;
        MyTile tile = tilesArray[y_search, x_search];

            if(tile.tile != null) {
                tm.SetTile(tile.tilePosition, newTile);
                
                TilemapRenderer renderer = tm.GetComponent<TilemapRenderer>();
               
            }
    }

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
    

    private bool checkIfTilemap(Vector3Int gridPosition)
    {
        
        if (gridPosition.y - tm.cellBounds.yMin >= 0  && gridPosition.y - tm.cellBounds.yMin < tm.cellBounds.size.y
            && gridPosition.x - tm.cellBounds.xMin >= 0 && gridPosition.x - tm.cellBounds.xMin < tm.cellBounds.size.x) {
            return true;
        }
        return false;
    }

    public void selectObject(MarketItemController selector_)
    {
        selector = selector_;
        Debug.Log("SELECTED");

    }

    void OnMouseDown()
    {
        if(selector == null)
        {
            return;
        }
        selector.unselectObject();
        if (selector.getObjectType() == MarketManager.Items.CANNON)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            Vector3Int gridPosition = tm.WorldToCell(mouseWorldPos);

            int x_search = gridPosition.x - x_min;
            int y_search = gridPosition.y - y_min;
            MyTile tile = tilesArray[y_search, x_search];

            if (tile.occupied)
            {
                return;
            }

            Vector3 placingPosition = tm.CellToWorld(gridPosition);

            placingPosition.x += 0.1f;
            placingPosition.y += 0.1f;
            placingPosition.z = 0.0f;

            if (checkIfTilemap(gridPosition))
            {
                if (tilesArray[y_search, x_search].tile == landTile)
                {
                    //tm.SetTile(tile.tilePosition, greyTile);
                    tilesArray[y_search, x_search].occupied = true;
                    selector.putObject(placingPosition);
                    selector = null;
                }
            }
        }
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
          /*  if(!checkIfTilemap(gridPosition))
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

}
