using UnityEngine;
using System.Collections.Generic;

public class EnemyWave : MonoBehaviour
{
    public GameObject shipPrefab;
    public Transform spawnPoint;
    private float minY = -8f;  
    private float maxY = 0.5f;   // manually adjust
    public float spaceMultiplier = 2f;  

    public void SpawnEnemies(List<int> drawnCards)
    {
        int numberOfBoats = drawnCards.Count;


        float yOffset = ((maxY - minY) / (numberOfBoats + 1)) * spaceMultiplier;  

        for (int i = 0; i < numberOfBoats; i++)
        {
            int card = drawnCards[i];

            // Calculate the Y position for each boat
            float yPos = minY + (i + 1) * yOffset;

            // Set the spawn position with the calculated Y position keeping X the same as the spawn point
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x, yPos, spawnPoint.position.z);

            Instantiate(shipPrefab, spawnPosition, spawnPoint.rotation);

            Debug.Log("Spawning boat at Y position: " + yPos + " for card value: " + card);
        }
    }
}
