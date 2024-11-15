using UnityEngine;
using System.Collections.Generic;

public class EnemyWave : MonoBehaviour
{
    public GameObject shipPrefab;
    public GameObject mermaidPrefab;
    public GameObject krakenPrefab;
    public GameObject sharkPrefab;
    public GameObject captainShipPrefab;
    public Transform spawnPoint;
    private float minY = -8f;
    private float maxY = 0.5f;
    public float spaceMultiplier = 2f;
    private float minX;
    private float xOffset = 7.0f;
    List<Transform> enemies = new List<Transform>();

    private static EnemyWave _instance;

    public static EnemyWave Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new EnemyWave();
                _instance = FindObjectOfType<EnemyWave>();
                if (_instance == null)
                {
                    Debug.LogError("No instance of MySingleton found in the scene!");
                }
            }
            return _instance;
            
        }
    }

    private void Awake()
    {
        // Ensure there is only one instance
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject); // Destroy any duplicate
            return;
        }

        _instance = this;
    }
    private void Update()
    {
        if (GameManager.instance.currentGameState == GameState.GS_BATTLE && areAllEnemiesDefeated())
        {
            Debug.Log("All enemies defeated. Ending wave.");
            GameManager.instance.Wait();
            StartCoroutine(GameManager.instance.WaveOver());
        }
        else
        {
//            Debug.Log("Enemies remaining: " + enemies.Count);
        }
    }



    public void RemoveEnemy(Transform enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            Debug.Log("Removed enemy from EnemyWave. Remaining enemies: " + enemies.Count);
        }
    }

    public List<Transform> getEnemies()
    {
        return this.enemies;
    }

    public void AddEnemy(Transform enemy)
    {
        enemies.Add(enemy);   
    }
    private bool areAllEnemiesDefeated()
    {
        enemies.RemoveAll(enemy => enemy == null); // Clean up destroyed enemies
        return enemies.Count == 0;
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

            if (card == 13) 
            {
                xPos = minX + xOffset * 0.02f; 
                spawnPosition = new Vector3(xPos, yPos, spawnPoint.position.z);
                newEnemy = Instantiate(captainShipPrefab, spawnPosition, spawnPoint.rotation);
                Transform captainShip = newEnemy.transform.Find("CaptainShip");
                enemies.Add(captainShip);
                if(captainShip != null)
                {
                    Debug.Log("Instantiated captain ship");
                    CaptainShip captainShip2 = captainShip.GetComponent<CaptainShip>();
      
                }           
            }
            else if (card == 11)
            {
                newEnemy = Instantiate(sharkPrefab, spawnPosition, spawnPoint.rotation);
                Debug.Log("Spawning shark at position: " + spawnPosition + " for card value: " + card);
                enemies.Add(newEnemy.transform.Find("Shark"));
            }
            else if (card == 12)
            {
                newEnemy = Instantiate(mermaidPrefab, spawnPosition, spawnPoint.rotation);
                Debug.Log("Spawning mermaid at position: " + spawnPosition + " for card value: " + card);
                enemies.Add(newEnemy.transform.Find("Mermaid"));
            }
            else if (card == 14)
            {
                newEnemy = Instantiate(krakenPrefab, spawnPosition, spawnPoint.rotation);
                Debug.Log("Spawning kraken at position: " + spawnPosition + " for card value: " + card);
                enemies.Add(newEnemy.transform.Find("Kraken"));
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