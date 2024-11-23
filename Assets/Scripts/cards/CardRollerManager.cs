

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Random = UnityEngine.Random;


public class CardRollManager : MonoBehaviour
{
    public Sprite[] cardSprites;  // Array to hold card sprites (2-10, A, J, Q, K)
    public Sprite bonusHPCardSprite;
    public Image cardDisplay;
    public Transform cardColumn;  // Vertical Layout Group for card placement
    public GameObject cardPrefab; // Prefab for dynamically instantiated cards
    public List<int> drawnCards = new List<int>();
    public EnemyWave enemyWave = null;
    private int hp = 50;  // Target HP
    private int drawnTotal = 0;  // Total drawn value so far
    private int randomCardIndex = 0;
    private int biggestCardIndex = 8;
    private bool rollingCards = true; // Flag to control when to stop rolling
    AudioSource audioSource;
    [SerializeField] protected AudioClip cardFlip;
    [SerializeField] protected AudioClip krakenOmen;

    public Font customFont; // Reference to the custom TMP font asset


    //bool rolled = false;


    void Start()
    {
        // Automatically roll cards when the game starts with a delay between each roll
        StartRolling();
    }


    private void Update()
    {
        // Existing GameManager logic untouched
        //if(GameManager.instance.currentGameState == GameState.GS_PREPARE && !rolled)
        //{
        //    rolled = true;
        //  StartRolling();
        //}
    }


    public void setTotalHp(int totalHp)
    {
        this.hp = totalHp;
    }

    //Let GameManager set who's the biggest enemy that we can pull from the deck.
    public void setBiggestEnemyValue(int biggestEnemyValue)
    {
        int constrainedBiggestEnemyValue;
        if (biggestEnemyValue > 14)
        {
            constrainedBiggestEnemyValue = 14;
        }
        else if (biggestEnemyValue < 2)
        {
            constrainedBiggestEnemyValue = 2;
        }
        else
        {
            constrainedBiggestEnemyValue = biggestEnemyValue;
        }

        //
        biggestCardIndex = constrainedBiggestEnemyValue - 2;
    }

    public int getBiggestEnemyValue()
    {
        return biggestCardIndex + 2;
    }


    public void ResetCardRoller()
    {
        drawnCards.Clear();
        drawnTotal = 0;
        rollingCards = true;
        cardDisplay.gameObject.SetActive(true);
    }


    public void ClearCards()
    {
        // Hide the CardColumn
        cardColumn.gameObject.SetActive(false);

        // Clear all instantiated cards from the column
        foreach (Transform child in cardColumn)
        {
            Destroy(child.gameObject);
        }

        // Clear data
        drawnCards.Clear();
        drawnTotal = 0;

        Debug.Log("Card column cleared and hidden.");
    }



    public void UpdateCardVisibility(bool isVisible)
    {
        // Update visibility of the CardDisplay
        cardDisplay.gameObject.SetActive(isVisible);


        // Update visibility of the CardColumn
        cardColumn.gameObject.SetActive(isVisible);


        Debug.Log("Card visibility updated: " + isVisible);
    }


    public void StartRolling()
    {
        // Ensure the CardColumn is visible at the start of rolling
        cardColumn.gameObject.SetActive(true);


        // Reset everything for the new roll
        ResetCardRoller();


        // Start the rolling coroutine
        StartCoroutine(StartCardRolling());
    }




    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    IEnumerator StartCardRolling()
    {
        GameManager.instance.Wait();


        // Roll cards without displaying them
        while (drawnTotal < hp && rollingCards)
        {

            RollCard();
            yield return null; // No delay between rolls in the logic
        }


        Debug.Log("Finished rolling cards. Total HP reached: " + drawnTotal);
        StartCoroutine(ShowCards()); // Begin showing cards after rolling
    }


    IEnumerator ShowCards()
    {
        foreach (int cardValue in drawnCards)
        {
            int cardIndex = cardValue - 2; // Convert card value to sprite index


            cardDisplay.sprite = cardSprites[cardIndex];
            cardDisplay.gameObject.SetActive(true); // Ensure the CardDisplay is visible
            audioSource.PlayOneShot(cardFlip, audioSource.volume);
            Debug.Log("Showing Card in Display: " + cardValue);



            yield return new WaitForSeconds(1.0f);

            //Kraken has a special sound for giving boss vibes.
            if (cardValue == 14) { audioSource.PlayOneShot(krakenOmen); }

            GameObject newCard = Instantiate(cardPrefab, cardColumn); // Create a new card
            Image cardImage = newCard.GetComponent<Image>();
            if (cardImage != null)
            {
                cardImage.sprite = cardSprites[cardIndex];
            }


            Debug.Log("Moved Card to Column: " + cardValue);

            // Hide the CardDisplay so it looks like the card is moved to the side. 
            cardDisplay.gameObject.SetActive(false);


            // Wait before showing the next card
            yield return new WaitForSeconds(0.5f);
        }

        yield return showBonusHPCard();

        // Hide the CardDisplay after all cards are moved
        cardDisplay.gameObject.SetActive(false);


        // Notify the game that all cards are done
        enemyWave.SpawnEnemies(drawnCards);
        GameManager.instance.Prepare();
    }

    private object showBonusHPCard()
    {
        cardDisplay.gameObject.SetActive(true); // Ensure the CardDisplay is visible
        cardDisplay.sprite = bonusHPCardSprite;
        audioSource.PlayOneShot(cardFlip, audioSource.volume);

        //Also show the round's bonus HP card.
        GameObject bonusHPCard = Instantiate(cardPrefab, cardColumn); // Create a new card
        Image bonusHPCardImage = bonusHPCard.GetComponent<Image>();
        if (bonusHPCardImage != null)
        {
            bonusHPCardImage.sprite = bonusHPCardSprite;
        }

        // Create a new Text component
        GameObject textObject = new GameObject("BonusHPText");
        textObject.transform.SetParent(bonusHPCard.transform);
        textObject.transform.localPosition = Vector3.zero; // Center the text object

        // Add and configure the Text component
        Text bonusHPCardText = textObject.AddComponent<Text>();
        if (EnemyManager.Instance != null)
        {

            bonusHPCardText.text = "+" + EnemyManager.Instance.HealthAddition;
        }
        else
        {
            bonusHPCardText.text = "Cannot Get EnemyManager";
            bonusHPCardText.fontSize = 24;
        }
        bonusHPCardText.font = customFont; // Assign the custom font
        bonusHPCardText.fontSize = 60; // Adjust the font size as needed
        bonusHPCardText.alignment = TextAnchor.MiddleCenter;
        bonusHPCardText.rectTransform.sizeDelta = bonusHPCard.GetComponent<RectTransform>().sizeDelta; // Make sure it covers the card

        bonusHPCardText.color = new Color(55f / 255f, 55f / 255f, 71f / 255f);

        // Wait before showing the next card
        return new WaitForSeconds(0.5f);
    }


   void RollCard()
{
        audioSource.PlayOneShot(cardFlip, audioSource.volume);

        int remainingHP = hp - drawnTotal;

 
        if (remainingHP >= 2 && remainingHP <= biggestCardIndex + 2 )
        {
            randomCardIndex = remainingHP - 2;
            drawnCards.Add(remainingHP);
            drawnTotal += remainingHP;
            Debug.Log("Forced exact match. Rolled Card: " + remainingHP + " | Total HP: " + drawnTotal);
            return;
        }

        List<int> validCardIndices = new List<int>();
        for (int i = 0; i <= biggestCardIndex; i++)
        {
            int cardValue = i + 2; 
            if (cardValue <= remainingHP) 
            {
                validCardIndices.Add(i);
            }
        }

        // If no valid cards remain, force an exact match
        if (validCardIndices.Count == 0)
        {
            randomCardIndex = remainingHP - 2;
            drawnCards.Add(remainingHP);
            drawnTotal += remainingHP;
            Debug.Log("Forced exact match. Rolled Card: " + remainingHP + " | Total HP: " + drawnTotal);
            return;
        }

        // Prioritize bigger cards using weighted random selection
        int selectedIndex = WeightedRandom(validCardIndices);

        // Add the selected card value to the drawn cards
        randomCardIndex = selectedIndex;
        int selectedCardValue = randomCardIndex + 2;

        drawnCards.Add(selectedCardValue);
        drawnTotal += selectedCardValue;

        Debug.Log($"Rolled Card: {selectedCardValue} | Total HP: {drawnTotal}");
    }



    int WeightedRandom(List<int> validIndices)
    {
        // Generate weights favoring bigger indices
        int totalWeight = 0;
        List<int> weights = new List<int>();

        foreach (int index in validIndices)
        {
            int weight = (index + 1) * 2; // Weight grows with index (e.g., 2, 4, 6, ...)
            weights.Add(weight);
            totalWeight += weight;
        }

        // Roll a random number within the total weight
        int randomWeight = Random.Range(0, totalWeight);

        // Determine the selected index based on weights
        int cumulativeWeight = 0;
        for (int i = 0; i < validIndices.Count; i++)
        {
            cumulativeWeight += weights[i];
            if (randomWeight < cumulativeWeight)
            {
                return validIndices[i];
            }
        }

        return validIndices[validIndices.Count - 1]; // Fallback to the last index
    }

}


