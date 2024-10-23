using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalShip : EnemyController
{
    private void Start()
    {
        //sets the starting destination on the closest waypoint and the final destination as the rum
        this.health = 5;
        this.speed = 1.5f;
        Prepare();
       
    }

    public override void Move()
    {
        // implement the movements of "normal" boats
        base.Move();
    }
    /*
    public override IEnumerator Die()
    {
        // Add additional logic for when a normal boat dies
        Debug.Log("Normal boat has been destroyed.");
        base.Die(); // Call base class's Die() to destroy the object
        yield return new WaitForSeconds(0);
    }*/
}
