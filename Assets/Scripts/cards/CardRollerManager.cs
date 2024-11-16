

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class CardRollManager : MonoBehaviour
{
    public Sprite[] cardSprites;  // Array to hold card sprites (2-10, A, J, Q, K)
    public Image cardDisplay;
    public Transform cardColumn;  // Vertical Layout Group for card placement
    public GameObject cardPrefab; // Prefab for dynamically instantiated cards
    public List<int> drawnCards = new List<int>();
    public EnemyWave enemyWave = null;
    private int hp = 50;  // Target HP
    private int drawnTotal = 0;  // Total drawn value so far
    private int randomCardIndex = 0;
    private bool rollingCards = true; // Flag to control when to stop rolling
    AudioSource audioSource;
    [SerializeField] protected AudioClip cardFlip;


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


    public void ResetCardRoller()
    {
        drawnCards.Clear();
        drawnTotal = 0;
        rollingCards = true;
        cardDisplay.gameObject.SetActive(true);
    }


    public void ClearCards()
    {
        // Hide the CardDisplay
        cardDisplay.gameObject.SetActive(false);
        cardColumn.gameObject.SetActive(false);




        // Clear all cards from the CardColumn
        foreach (Transform child in cardColumn)
        {
            Destroy(child.gameObject);
            //child.gameObject.SetActive(false);
        }


        // Clear the drawnCards list
        drawnCards.Clear();
        drawnTotal = 0;


        Debug.Log("Cards cleared!");
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


            // Step 1: Show the card in the CardDisplay
            cardDisplay.sprite = cardSprites[cardIndex];
            cardDisplay.gameObject.SetActive(true); // Ensure the CardDisplay is visible
            audioSource.PlayOneShot(cardFlip, audioSource.volume);
            Debug.Log("Showing Card in Display: " + cardValue);


            // Wait for the card to be visible in the CardDisplay
            yield return new WaitForSeconds(1.0f);


            // Step 2: Move the card to the CardColumn
            GameObject newCard = Instantiate(cardPrefab, cardColumn); // Create a new card
            Image cardImage = newCard.GetComponent<Image>();
            if (cardImage != null)
            {
                cardImage.sprite = cardSprites[cardIndex]; // Set the same sprite as the preview
            }


            Debug.Log("Moved Card to Column: " + cardValue);


            // Wait before showing the next card
            yield return new WaitForSeconds(0.5f);
        }


        // Hide the CardDisplay after all cards are moved
        cardDisplay.gameObject.SetActive(false);


        // Notify the game that all cards are done
        enemyWave.SpawnEnemies(drawnCards);
        GameManager.instance.Prepare();
    }




    void RollCard()
    {
        audioSource.PlayOneShot(cardFlip, audioSource.volume);


        int remainingHP = hp - drawnTotal;


        // Special case: If remaining HP matches one of the possible card values, force that value
        if (remainingHP >= 2 && remainingHP <= 14)
        {
            randomCardIndex = remainingHP - 2;  // Force the card value to exactly match the remaining HP
            drawnCards.Add(remainingHP);
            drawnTotal += remainingHP;


            Debug.Log("Forced exact match. Rolled Card: " + remainingHP + " | Total HP: " + drawnTotal);
            return;
        }


        do
        {
            randomCardIndex = Random.Range(0, cardSprites.Length);
        }
        while ((randomCardIndex + 2) > remainingHP);


        drawnCards.Add(randomCardIndex + 2);
        drawnTotal += (randomCardIndex + 2); // Add card value (index + 2)


        Debug.Log("Rolled Card: " + (randomCardIndex + 2) + " | Total HP: " + drawnTotal);
    }
}




