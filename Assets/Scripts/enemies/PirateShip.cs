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
        Prepare();

    }

  

    public void setHealth(int hp)
    {
        base.health = hp;
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


