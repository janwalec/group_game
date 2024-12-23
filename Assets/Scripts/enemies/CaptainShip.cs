using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CaptainShip : EnemyController
{
    private bool isSlowed = false;
    public GameObject pirateShipPrefab;
    private Vector3 spawnOffset = new Vector3(1f, -1f, 0f);  
    private float spawnInterval = 10f;
    private float initialDelay = 10f;   // Initial delay before starting the spawn routine
    private List<Transform> enemies;
    private EnemyWave wave;
    private GameObject shipPrefab;
    private int initHealth;

    public void Start()
    {
        base.health = 13;
        base.speed = 0.7f;
        base.Start();
        //base.ApplyHealthAddition();
        //base.ApplySpeedMultiplication();
        initHealth = health;
        Prepare();
        StartCoroutine(SpawnEnemyRoutine());  
    }

    
    public void StartSpawningEnemies()
    {
        StartCoroutine(SpawnEnemyRoutine());  
    }
    

    public override void Move()
    {
        base.Move();
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        // Wait until the game state is GS_BATTLE
        while (GameManager.instance.currentGameState != GameState.GS_BATTLE)
        {
            yield return null;  // Wait until the next frame before checking again
        }

        // Log when the game enters battle state
        Debug.Log("Game has entered GS_BATTLE state, starting initial delay...");

        // Initial delay after entering battle state
        yield return new WaitForSeconds(initialDelay);

        Debug.Log("Initial delay complete. Starting to spawn enemies...");

        while (health > 0)  // Continue spawning as long as CaptainShip is alive
        {
            Debug.Log("Spawning a new pirate ship at time: " + Time.time);
            SpawnEnemy();

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
            Vector3 spawnPosition = transform.position;
            GameObject newPirateShip = Instantiate(pirateShipPrefab, spawnPosition, Quaternion.identity);
            Transform pirateBoat = newPirateShip.transform.Find("EnemyShip");
            NormalShip normalShip = pirateBoat.GetComponent<NormalShip>();
            EnemyWave.Instance.AddEnemy(pirateBoat);

            if (normalShip != null)
            {
                GameObject[] remainingWaypoints = new GameObject[waypoints.Length - currentWaypoint];
                Array.Copy(waypoints, currentWaypoint, remainingWaypoints, 0, remainingWaypoints.Length);
                int childHealth = (int)(initHealth * 0.3f);//Get 20% of captainships initial HP.
                if (childHealth < 2)
                {
                    childHealth = 2;
                }
                normalShip.InitializeHealth(childHealth, false);
                normalShip.InitializeWaypoints(remainingWaypoints,0);

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
