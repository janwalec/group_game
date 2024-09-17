using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public float speed; // maybe not needed? 

    public virtual void Move()
    { // implement the logic for movement

        Debug.Log("Enemy is moving");
    }

    public virtual void TakeDamage(int dmg)
    {
        health -= dmg;

        if(health <= 0) {
            Die();
        }
    }
    
    public virtual void Die()
    {
        Debug.Log("Enemy has died.");
        Destroy(gameObject); // Remove enemy from the scene
    }
}
