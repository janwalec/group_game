using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptainShip : EnemyController
{
    private bool isSlowed = false;
    public GameObject pirateShipPrefab;
    private Vector3 spawnOffset = new Vector3(1f, -1f, 0f);  
    private float spawnInterval = 15f;  
    private float initialDelay = 10f;   // Initial delay before starting the spawn routine
    private List<Transform> enemies;
    private EnemyWave wave;
    private GameObject shipPrefab;

    public void Start()
    {
        base.health = 13;
        base.speed = 0.8f;
        base.ApplyHealthAddition();
        base.ApplySpeedMultiplication();
        Prepare();
        StartCoroutine(SpawnEnemyRoutine());  
    }

    

    public void StartSpawningEnemies()
    {
        StartCoroutine(SpawnEnemyRoutine());  
    }

    protected override IEnumerator SlowDown(float newSpeed)
    {
        if (newSpeed >= this.speed)
        {
            newSpeed = this.speed;
        }

        speed = newSpeed;  // Apply slowing effect factor to speed
        Debug.Log("Enemy speed: " + speed);
        yield return new WaitForSeconds(2f);  // Slow effect lasts for 3 seconds
    }

    public override void Move()
    {
        base.Move();
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        Debug.Log("SpawnEnemyRoutine coroutine started. Waiting for initial delay...");
        yield return new WaitForSeconds(initialDelay);

        Debug.Log("Initial delay complete. First ship instantiation starting now.");

        while (health > 0)  // Continue spawning as long as CaptainShip is alive
        {
            // Check if the game is in the battle state before spawning
            if (GameManager.instance.currentGameState == GameState.GS_BATTLE)
            {
                Debug.Log("Spawning a new pirate ship at time: " + Time.time);
                SpawnEnemy();
            }
            else
            {
                // Wait a short time before checking the game state again
                Debug.Log("Game is not in battle state. Pausing spawning...");
                yield return new WaitForSeconds(0.5f);
                continue; // Skip to the next iteration
            }

            // Wait for the spawn interval before attempting to spawn again
            yield return new WaitForSeconds(spawnInterval);
        }
    }


    public int GetHealth()
    {
        return base.health;
    }

    private void SpawnEnemy()
    {
        if (pirateShipPrefab != null)
        {
            Vector3 spawnPosition = transform.position + spawnOffset;
            GameObject newPirateShip = Instantiate(pirateShipPrefab, spawnPosition, Quaternion.identity);
            Transform pirateBoat = newPirateShip.transform.Find("EnemyShip");
            NormalShip normalShip = pirateBoat.GetComponent<NormalShip>();
            EnemyWave.Instance.AddEnemy(pirateBoat);
            if (normalShip != null)
            {
                normalShip.InitializeHealth(5);
                
            }
            else
            {
                Debug.LogError("NormalShip component not found on the PirateShip prefab.");
            }

            
        }
        else
        {
            Debug.LogError("PirateShip prefab is not assigned in the Inspector!");
        }
    }

}
