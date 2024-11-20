using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kraken : EnemyController
{
    private int threshold = 10;
    private bool canTakeDmg; 
    private void Start()
    {
        this.speed = 1.5f;
        health = 10;
        Prepare();
    }

    public override void Move()
    {

        base.Move();
    }

    public void checkDamage(int dmg)
    {
        if(dmg<threshold)
        {
            canTakeDmg = false;
        }
        else
        {
            canTakeDmg = true;
        }
    }

    public override void TakeDamage(int dmg)
    {
        checkDamage(dmg);
        
        if(canTakeDmg == true)
        {
            base.TakeDamage(dmg);
            
        }

    }

    /* public override void Die()
     {

         Debug.Log("Normal boat has been destroyed.");
         base.Die();
     }*/
}
