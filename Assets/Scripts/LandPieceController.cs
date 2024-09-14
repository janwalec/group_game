using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LandPieceController : MonoBehaviour
{
    [SerializeField]
    Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        /*
        GameObject new_weapon = ObjectPool.SharedInstance.GetPooledObject();
        new_weapon.transform.position = Vector3.zero;
        Debug.Log("here");
       
        Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = (int)mouse_pos.x;
        int y = (int)mouse_pos.y;
        int z = (int)mouse_pos.z;
        Debug.Log(tilemap.size.x);
        Debug.Log(tilemap.size.y);
        Debug.Log(tilemap.size.z);
        Vector3Int tilepos = tilemap.WorldToCell(mouse_pos);
        Tile t = (Tile)tilemap.GetTile(tilepos);
        Debug.Log(t.name);
        Debug.Log(t.transform.GetPosition());
        new_weapon.transform.position  = t.transform.GetPosition();
        new_weapon.SetActive(true);
        */
    }
}
