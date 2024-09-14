using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;

public struct My_Tile {
    public int y, x;
    public Vector3Int tilePosition;
    public TileBase tile;
    public My_Tile[] neighbours;

    public My_Tile(int y, int x, TileBase tile) {
        this.y = y;
        this.x = x;
        this.tilePosition = new Vector3Int(x, y, 0);
        this.tile = tile;
        this.neighbours  = new My_Tile[6];
    }
}


public class TIleMapGenerator : MonoBehaviour
{
    public Tilemap tm;
    public Tile newTile;

    private My_Tile[,] tilesArray;
    private int size_x = 0, size_y = 0;
    private int y_min, x_min, y_max, x_max;
    
    // Start is called before the first frame update
    void Start() {
        tm = GetComponent<Tilemap>();

        BoundsInt bounds = tm.cellBounds;
        this.size_x = bounds.size.x; 
        this.size_y = bounds.size.y;
        this.y_min = bounds.yMin;
        this.y_max = bounds.yMax;
        this.x_min = bounds.xMin;
        this.x_max = bounds.xMax;
        /*
        Debug.Log("size" + size_x.ToString() + " " + size_y.ToString());
        Debug.Log("min" + bounds.xMin.ToString() + " " + bounds.yMin.ToString());
        Debug.Log("max" + bounds.xMax.ToString() + " " + bounds.yMax.ToString());
        */

        tilesArray = new My_Tile[size_y, size_x];

        for (int i = bounds.yMin; i < bounds.yMax; i++) {
            for (int j = bounds.xMin; j < bounds.xMax; j++) {
                Vector3Int tilePosition = new Vector3Int(j, i, 0);
                TileBase tile = tm.GetTile(tilePosition);

                tilesArray[i - bounds.yMin, j - bounds.xMin] = new My_Tile(i, j, tile);
            }
        }   
        CreateTileNeighbours();
    }

    void CreateTileNeighbours() {
        
        for (int i = y_min; i < y_max; i++) {
            for (int j = x_min; j < x_max; j++) {
                int l = i - y_min;
                int k = j - x_min;
                My_Tile tile = tilesArray[l, k];
                if(l+1 < size_y && k-1 > 0)
                    tile.neighbours[0] = tilesArray[l+1, k-1];
                if(l+1 < size_y)
                    tile.neighbours[1] = tilesArray[l+1, k];
                if(k-1 > 0)
                    tile.neighbours[2] = tilesArray[l, k-1];
                if(k+1 < size_x)
                    tile.neighbours[3] = tilesArray[l, k+1];
                if(l-1 > 0 && k -1 > 0)
                    tile.neighbours[4] = tilesArray[l-1, k-1];
                if(l-1 > 0)
                    tile.neighbours[5] = tilesArray[l-1, k];

            }
        }
        
        string print = "";
        for (int i = 0; i < size_y; i++) {
            for(int j = 0; j < size_x; j++) {
                if(tilesArray[i,j].tile == null) {
                    print += "[.,.] ";
                } else {
                    print += "[";
                    print += tilesArray[i,j].y.ToString();
                    print += ",";
                    print += tilesArray[i,j].x.ToString();
                    print += "] ";
                }
            }
            print += "\n";
        }
        Debug.Log(print);
        */

    }


    void ChangeTiles(Vector3Int position)
    {
        int x_search = position.x - x_min;
        int y_search = position.y - y_min;
        My_Tile tile = tilesArray[y_search, x_search];

            if(tile.tile != null) {
                tm.SetTile(tile.tilePosition, newTile);
                
                TilemapRenderer renderer = tm.GetComponent<TilemapRenderer>();
                int originalOrder = renderer.sortingOrder;
                renderer.sortingOrder = originalOrder + 1;
                renderer.sortingOrder = originalOrder;
                tm.RefreshAllTiles();
            }
    }

    void DisplayNeighbours(Vector3Int position) {
        int x_search = position.x - x_min;  
        int y_search = position.y - y_min;
        My_Tile tile = tilesArray[y_search, x_search];
        
        if(tile.tile != null) {
            for (int i = 0; i < 6; i++) {
                if(tile.neighbours[i].tile != null) {
                    tm.SetTile(tile.neighbours[i].tilePosition, newTile);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = tm.WorldToCell(mouseWorldPos);
            gridPosition.z = 0;
            int xx = gridPosition.x;
            int yy = gridPosition.y;
            Debug.Log("y: " + yy.ToString() + "  x: " + xx.ToString());
            DisplayNeighbours(gridPosition);
            //ChangeTiles(gridPosition);
        }
    
    }
}
