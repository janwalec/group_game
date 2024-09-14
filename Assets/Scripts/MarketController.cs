using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MarketController : MonoBehaviour
{
    [SerializeField]
    Image cannonFrameImage;

    //[SerializeField]
   // Tilemap tilemap;
 
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CannonSelected()
    {
        cannonFrameImage.color = Color.red;
        GameObject new_weapon = ObjectPool.SharedInstance.GetPooledObject();
        new_weapon.transform.position = Vector3.zero;
        new_weapon.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("OnLand");
        new_weapon.SetActive(true);
        /*Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = (int)mouse_pos.x;
        int y = (int)mouse_pos.y;
        int z = (int)mouse_pos.z;
        Tile t = (Tile)tilemap.GetTile(new Vector3Int(x,y,z));
        t.transform.GetPosition();*/
    }
}
