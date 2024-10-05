using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EnemyWave : MonoBehaviour
{
    public GameObject shipPrefab;
    public Transform spawnPoint;
    private float minY;
    private float maxY;

    public void SpawnEnemies(List<int> drawnCards)
    {
        int numberOfBoats = drawnCards.Count;

        // Get the camera bounds in world space
        Camera mainCamera = Camera.main;
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane));

        // Set the minimum and maximum Y positions based on the camera bounds
        minY = bottomLeft.y;
        maxY = topRight.y;

        // Calculate the Y offset for evenly distributing boats
        float yOffset = (maxY - minY) / (numberOfBoats + 1);  // +1 to add padding

        for (int i = 0; i < numberOfBoats; i++)
        {
            int card = drawnCards[i];

            // Calculate the Y position for each boat
            float yPos = minY + (i + 1) * yOffset;  // Use (i + 1) to avoid placing boats right on the edge

            // Set the spawn position with the calculated Y position, keeping X the same as the spawn point
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x, yPos, spawnPoint.position.z);

            // Instantiate the ship at the new position
            Instantiate(shipPrefab, spawnPosition, spawnPoint.rotation);

            Debug.Log("Spawning boat at Y position: " + yPos + " for card value: " + card);
        }
    }
}
