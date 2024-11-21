using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : EnemyController
{
    private bool isSlowed = false;
    private void Start()
    {
        //sets the starting destination on the closest waypoint and the final destination as the rum
        base.health = 15;
        base.speed = 1.15f;
        ApplyHealthAddition();
        Prepare();
    }

    private void ApplyHealthAddition()
    {
        // Ensure EnemyManager exists
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
