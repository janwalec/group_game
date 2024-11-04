using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : EnemyController

{
    // Start is called before the first frame update
    void Start()
    {
        //sets the starting destination on the closest waypoint and the final destination as the rum
        //currentDestination = waypoints[currentWaypoint].GetComponentInParent<Transform>().position;
        //finalDestination = GameManager.instance.getRumPosition();
        this.health = 100;
        this.speed = 1.5f;
        changeText(health.ToString());
    }

    public override void Move()
    {
        // shark can't be slowed
        base.Move();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
