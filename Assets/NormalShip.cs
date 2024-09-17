using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalShip : Enemy
{
    private void Start()
    {
        health = 100;
        speed = 5f;
    }

    public override void Move()
    {
        // implement the movements of "normal" boats
        Debug.Log("Normal boat is moving.");
    }

    public override void Die()
    {
        // Add additional logic for when a normal boat dies
        Debug.Log("Normal boat has been destroyed.");
        base.Die(); // Call base class's Die() to destroy the object
    }
}
