using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.Collections;
using System.Collections.Generic;

public class CardRollManager : MonoBehaviour
{
    public Sprite[] cardSprites;  // Array to hold card sprites (2-10, A, J, Q, K)
    public Image cardDisplay;     
    public TMP_Text cardValueText; 
    public List<int> drawnCards = new List<int>();
    public EnemyWave enemyWave = null;
    private int hp = 50;  // Target HP
    private int drawnTotal = 0;  // Total drawn value so far
    private int randomCardIndex = 0; 
    private bool rollingCards = true; // Flag to control when to stop rolling

    void Start()
    {
        // automatically roll cards when the game starts with a delay between each roll
        StartCoroutine(StartCardRolling());
    }

    IEnumerator StartCardRolling()
    {
      
        while (drawnTotal < hp && rollingCards)
        {
            RollCard();
            yield return new WaitForSeconds(0.5f);  // qait for 2 seconds before the next roll
        }

       
        Debug.Log("Finished rolling cards. Total HP reached: " + drawnTotal);
        enemyWave.SpawnEnemies(drawnCards);
    }

    void RollCard()
    {

        int remainingHP = hp - drawnTotal;

     
        do
        {
            randomCardIndex = Random.Range(0, cardSprites.Length);
        }
        while ((randomCardIndex + 2) > remainingHP);  

     
        drawnCards.Add(randomCardIndex+2);
        drawnTotal += (randomCardIndex + 2);  

       
        cardDisplay.sprite = cardSprites[randomCardIndex];

        // remove this 
        if (cardValueText != null)
        {
            cardValueText.text = "Card Value: " + (randomCardIndex + 2);  // Add 2 since index 0 = card value 2
        }

        Debug.Log("Rolled Card: " + (randomCardIndex + 2) + " | Total HP: " + drawnTotal);
    }
}
