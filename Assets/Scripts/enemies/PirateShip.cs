using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class NormalShip : EnemyController
{
    private void Start()
    {
        base.speed = 1f;
        base.ApplySpeedMultiplication();
        this.Prepare();

    }



    protected override void Prepare()
    {
        //waypoints = EnemyPathManager.Instance.getRandomPath();
        //currentDestination = waypoints[currentWaypoint].GetComponentInParent<Transform>().position;


        finalDestination = GameManager.instance.getRumPosition();
        changeText(health.ToString());
        priceForKill = health;

        // If the Slider is found, set its maxValue
        if (healthBar != null)
        {
            healthBar.maxValue = health;
            healthBar.value = health; // Initialize the slider with current health value
        }
        else
        {
            Debug.LogError("Slider component not found in children!");
        }

    }

    public void setHealth(int hp)
    {
        base.health = hp;
    }

    public override void Move()
    {

        base.Move();
    }

    public int GetHealth()
    {
        return base.health;
    }

    public void InitializeWaypoints(GameObject[] newWaypoints, int startPoint)
    {
        waypoints = newWaypoints;
        currentWaypoint = startPoint;
        currentDestination = waypoints[currentWaypoint].transform.position;
    }
    public void InitializeHealth(int card_value)
    {
        health = card_value;
        base.health = health;
        changeText(health.ToString());
        base.ApplyHealthAddition();
        //Debug.Log("Initialized health with card value " + card_value + ". New health: " + health);
    }


    /* public override void Die()
     {

         Debug.Log("Normal boat has been destroyed.");
         base.Die();
     }*/
}


