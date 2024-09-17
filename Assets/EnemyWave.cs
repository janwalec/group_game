using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// algorithm to initalize how many ships we have and what the type is
public class EnemyWave : MonoBehaviour 
{
    //public string enemyType; // normal/advanced
    public int waveHP; 
    private List<int> drawnCards; // store all the numbers we get when drawing cards
    public NormalShip ship;
    public Transform spawnPoint;


    public void SetDrawnCards(List<int> cards)
    {
        drawnCards = new List<int>(cards);
        Debug.Log("Cards receivied from a wave!");

        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        if(drawnCards == null)
        {
            Debug.LogError("No cards were drawn!");
        }

        foreach(int card in drawnCards)
        {
            if (card >= 1 && card <= 10)
            {
                Debug.Log("Spawning normal boat for card value: " + card);
                Instantiate(ship, spawnPoint.position, spawnPoint.rotation);
            }

            if(card >= 11 && card<=14 )
            {
                Debug.Log("Spawning mermaid");
                // Instantiate
            }

            drawnCards.Clear();
        }
    }

}
