using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mermaid : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        health = 200;
        speed = 3f;
    }

   public override void Move()
   {
        Debug.Log("Advanced ship is moving");
   }

    public override void Die()
    { 
        Debug.Log("Destroying advanced ship");
        base.Die();
    }
}
