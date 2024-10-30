using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.Collections;
using System.Collections.Generic;

public class CardRollManager : MonoBehaviour
{
    public Sprite[] cardSprites;  // Array to hold card sprites (2-10, A, J, Q, K)
    public Image cardDisplay;     
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
        // automatically roll cards when the game starts with a delay between each roll
        StartCoroutine(StartCardRolling());
    }

    private void Update()
    {
       //if(GameManager.instance.currentGameState == GameState.GS_PREPARE && !rolled)
        //{
        //    rolled = true;
        //  StartRolling();
        //}
    }

    public void StartRolling()
    {
        StartCoroutine(StartCardRolling());
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    IEnumerator StartCardRolling()
    {
        
            while (drawnTotal < hp && rollingCards)
            {
                RollCard();
                yield return new WaitForSeconds(1.5f);  // qait for 1 seconds before the next roll
            }


            Debug.Log("Finished rolling cards. Total HP reached: " + drawnTotal);
            enemyWave.SpawnEnemies(drawnCards);
            cardDisplay.gameObject.SetActive(false);
        
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
            cardDisplay.sprite = cardSprites[randomCardIndex];

            Debug.Log("Forced exact match. Rolled Card: " + remainingHP + " | Total HP: " + drawnTotal);
            return;
        }

        do
        {
            randomCardIndex = Random.Range(0, cardSprites.Length);
        }
        while ((randomCardIndex + 2) > remainingHP);  

     
        drawnCards.Add(randomCardIndex + 2);
        drawnTotal += (randomCardIndex + 2);  // Add card value (index + 2)

        // Display the card sprite
        cardDisplay.sprite = cardSprites[randomCardIndex];

        Debug.Log("Rolled Card: " + (randomCardIndex + 2) + " | Total HP: " + drawnTotal);
    }

}
