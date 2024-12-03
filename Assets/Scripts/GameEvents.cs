using System.Collections;
using System.Collections.Generic;
using System;

public static class GameEvents
{
    // Define the event using Action<int> where int is the points for the killed enemy
    public static event Action<int> OnEnemyKilled;

    // Method to invoke the event
    public static void EnemyKilled(int points)
    {
        // Invoke the event if there are any subscribers
        OnEnemyKilled?.Invoke(points);
    }
}

