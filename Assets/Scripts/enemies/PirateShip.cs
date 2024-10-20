using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalShip : EnemyController
{
    private void Start()
    {
        //sets the starting destination on the closest waypoint and the final destination as the rum
        currentDestination = waypoints[currentWaypoint].GetComponentInParent<Transform>().position;
        finalDestination = GameManager.instance.getRumPosition();
        this.health = 100;
        this.speed = 1.5f;
        changeText(health.ToString());
    }

    public override void Move()
    {
        // implement the movements of "normal" boats
        base.Move();
    }

    public override void Die()
    {
        // Add additional logic for when a normal boat dies
        Debug.Log("Normal boat has been destroyed.");
        base.Die(); // Call base class's Die() to destroy the object
    }
}
