using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class NormalShip : EnemyController
{
    private void Start()
    {
        base.speed = 1f;
        Prepare();

    }

    private void ApplyHealthAddition()
    {
        
        if (EnemyManager.Instance != null)
        {
            int additionalHealth = EnemyManager.Instance.HealthAddition;
            base.health += additionalHealth; // Add the global health addition to the base health
            Debug.Log($"{name} Final Health: {base.health}");
        }
        else
        {
            Debug.LogWarning("EnemyManager is not present in the scene.");
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

    public  int GetHealth()
    {
        return base.health;
    }
    public void InitializeHealth(int card_value)
    {
        health = 5 + card_value;
        base.health = health;
        changeText(health.ToString());
        ApplyHealthAddition();
        //Debug.Log("Initialized health with card value " + card_value + ". New health: " + health);
    }


   /* public override void Die()
    {

        Debug.Log("Normal boat has been destroyed.");
        base.Die();
    }*/
}
