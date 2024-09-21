using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CardClickHandler : MonoBehaviour
{
    public GameObject cardValueTextObject;
    public List<int> drawnCards = new List<int>();
    public EnemyWave enemyWave = null;
    public int hp = 50;
    public int drawnTotal = 0;
    public int randomCardValue = 0;

    public void OnCardClick()
    {
        if (cardValueTextObject != null)
        {
            // Get the TextMeshProUGUI component from the GameObjet
            TextMeshProUGUI cardValueText = cardValueTextObject.GetComponent<TextMeshProUGUI>();

            if (drawnTotal < hp)
            {
                if (cardValueText != null)
                {
                    int remainingHP = hp - drawnTotal;
                    // Keep drawing a card until we get one that fits within the remaining HP
                    do
                    {
                        randomCardValue = Random.Range(1, 14);
                    }
                    while (randomCardValue > remainingHP);  // Redraw if the card exceeds the remaining HP


                    // Generate a random number between 1 and 14
                    drawnTotal += randomCardValue;
                    drawnCards.Add(randomCardValue);
                    cardValueText.text = randomCardValue.ToString();
                    Debug.Log("Card Value: " + randomCardValue + "Total value:" + drawnTotal);


                    if (drawnTotal >= hp)
                    {
                        Debug.Log("Target HP reached. Preparing to spawn boats.");

                    }
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI component not found on CardValueText GameObject!");
                }
            }
        }
        else
        {
            Debug.LogError("CardValueText GameObject is not assigned in the Inspector.");
        }

        if(drawnTotal == hp)
        {
            foreach (var card_value in drawnCards)
            {
                Debug.Log("Num. of boats that are spawning:" + drawnCards.Count);
                Debug.Log("card_value:" + card_value);
            }

            enemyWave.SpawnEnemies(drawnCards);
        }
       
    }
}
