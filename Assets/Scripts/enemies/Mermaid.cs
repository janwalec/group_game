using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mermaid : EnemyController
{
    // Start is called before the first frame update
    private void Start()
    {
        health = 200;
        speed = 3f;
        changeText(health.ToString());
    }

   public override void Move()
   {
        base.Move();
   }

    public override void Die()
    { 
        Debug.Log("Destroying advanced ship");
        base.Die();
    }
}
