using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalShip : EnemyController
{
    private void Start()
    {
        //sets the starting destination on the closest waypoint and the final destination as the rum
        this.speed = 1.5f;
        Prepare();
    }

    public void setHealth(int hp)
    {
        this.health = hp;
    }

    public override void Move()
    {

        base.Move();
    }

    public  int GetHealth()
    {
        return base.health;
    }
    public void InitializeHealth(int card_value)
    {
        health = 5 + card_value;
        changeText(health.ToString());
        //Debug.Log("Initialized health with card value " + card_value + ". New health: " + health);
    }


   /* public override void Die()
    {

        Debug.Log("Normal boat has been destroyed.");
        base.Die();
    }*/
}
