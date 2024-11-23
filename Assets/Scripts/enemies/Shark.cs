using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : EnemyController
{
    private bool isSlowed = false;
    private void Start()
    {
        //sets the starting destination on the closest waypoint and the final destination as the rum
        base.health = 11;
        base.speed = 1.15f;
        base.ApplyHealthAddition();
        base.ApplySpeedMultiplication();
        Prepare();
    }

   
    public override void Move()
    {

        base.Move();
    }

    public int GetHealth()
    {
        return base.health;
    }
   
    protected override IEnumerator SlowDown(float newSpeed)
    {
        if(isSlowed == false)
        {
            Debug.Log("Enemy is a shark, can't be slowed!");
        }
        Debug.Log("Shark speed: " + speed);
        yield return new WaitForSeconds(3f);  
    }

    /* public override void Die()
     {

         Debug.Log("Normal boat has been destroyed.");
         base.Die();
     }*/
}
