using UnityEngine;
using System.Collections.Generic;

public class EnemyWave : MonoBehaviour
{
    public GameObject shipPrefab;
    public GameObject mermaidPrefab;
    public Transform spawnPoint;
    private float minY = -8f;
    private float maxY = 0.5f;
    public float spaceMultiplier = 2f;
    private float minX;
    private float xOffset = 7.0f;
    List<Transform> enemies = new List<Transform>();

    private void Update()
    {
        if (GameManager.instance.currentGameState == GameState.GS_BATTLE && areAllEnemiesDefeated())
        {
            GameManager.instance.Prepare();
            GameManager.instance.WaveOver();
        }
    }

    private bool areAllEnemiesDefeated()
    {
        int counter = 0;
        foreach(Transform enemy in enemies)
        {
            if(enemy != null )
            {
                counter++;
            }
        }
        
        return counter == 0;
    }
    public void SpawnEnemies(List<int> drawnCards)
    {
        int numberOfEnemies = drawnCards.Count;
        float yOffset = ((maxY - minY) / (numberOfEnemies + 1)) * spaceMultiplier;

        minX = spawnPoint.position.x;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int card = drawnCards[i];

            // Calculate positions for each boat or mermaid
            float yPos = minY + (i + 1) * yOffset;
            float xPos = minX + (i + 1) * xOffset;
            Vector3 spawnPosition = new Vector3(xPos, yPos, spawnPoint.position.z);

            GameObject newEnemy;

            // Spawn based on card value: mermaid for special cards, ship otherwise
            if (card >=9 && card<=15)
            {
                newEnemy = Instantiate(mermaidPrefab, spawnPosition, spawnPoint.rotation);
                Debug.Log("Spawning mermaid at position: " + spawnPosition + " for card value: " + card);
                enemies.Add(newEnemy.transform.Find("Mermaid"));
            }
            else
            {
                newEnemy = Instantiate(shipPrefab, spawnPosition, spawnPoint.rotation);

                // Get the NormalShip component from the Pirate_Boat child
                Transform pirateBoat = newEnemy.transform.Find("Pirate_Boat");
                enemies.Add(pirateBoat);
                if (pirateBoat != null)
                {
                    NormalShip normalShip = pirateBoat.GetComponent<NormalShip>();
                    if (normalShip != null)
                    {
                        normalShip.InitializeHealth(card);
                        Debug.Log($"Spawned ship with card value {card}. Assigned health: {normalShip.GetHealth()} at position: {spawnPosition}");
                    }
                    else
                    {
                        Debug.LogError("NormalShip script not found on the Pirate_Boat child!");
                    }
                }
                else
                {
                    Debug.LogError("Pirate_Boat child not found on the instantiated EnemyShip prefab!");
                }
                
            }
        }
    }
}
