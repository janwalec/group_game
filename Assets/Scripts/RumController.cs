using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumController : MonoBehaviour
{

    void Start()
    {
        //occupy tiles so that a cannon or a modifier cannot be placed there
        GameManager.instance.getTilemap().occupyTile(transform.position);
    }


    void Update()
    {
        
    }
}
