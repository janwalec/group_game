

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
        cardColumn = GameObject.Find("CardColumn").GetComponent<RectTransform>();
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
            // Generate a random Z-rotation offset between -10 and +10 degrees
            float randomZRotation = Random.Range(-10f, 10f);

            // Apply the new Z-rotation to the card (keep the existing rotation on X and Y)
            newCard.transform.rotation = Quaternion.Euler(newCard.transform.rotation.eulerAngles.x, newCard.transform.rotation.eulerAngles.y, randomZRotation);


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
        bonusHPCardText.fontSize = 48; // Adjust the font size as needed
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


        // Special case: If remaining HP matches one of the possible card values, force that value
        //Debug.Log("Biggest enemy possible by index: "+ biggestCardIndex);
        if (remainingHP >= 2 && remainingHP <= biggestCardIndex + 2)
        {
            randomCardIndex = remainingHP - 2;  // Force the card value to exactly match the remaining HP
            drawnCards.Add(remainingHP);
            drawnTotal += remainingHP;


            Debug.Log("Forced exact match. Rolled Card: " + remainingHP + " | Total HP: " + drawnTotal);
            return;
        }


        do
        {
            randomCardIndex = Random.Range(0, biggestCardIndex);
        }
        while ((randomCardIndex + 2) > remainingHP);


        drawnCards.Add(randomCardIndex + 2);
        drawnTotal += (randomCardIndex + 2); // Add card value (index + 2)


        Debug.Log("Rolled Card: " + (randomCardIndex + 2) + " | Total HP: " + drawnTotal);
    }
}



