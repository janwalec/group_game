using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kraken : EnemyController
{
    private int threshold = 8;
    private bool canTakeDmg;
    public AudioClip krakenDeflectSound;
    private void Start()
    {
        base.speed = 1f;
        base.health = 20;
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
        else
        {
            audioSource.PlayOneShot(krakenDeflectSound);
        }
    }

    /* public override void Die()
     {

         Debug.Log("Normal boat has been destroyed.");
         base.Die();
     }*/
}
