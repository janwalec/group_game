using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RumController : MonoBehaviour
{
    double HPLoss = 0;
    int HPInt = 10;

    [SerializeField] private LayerMask enemyMask;
    public Canvas canvas;
    private float range = 2.0f;
    AudioSource audioSource;
    [SerializeField] AudioClip onDamageSound;
    [SerializeField] AudioClip onLossSound;
    private float krakenMultiplier = 2.0f;  // Kraken consumes rum at twice the normal rate
    private float damageTimer = 0f;
    private Slider healthBar;
    //SFX
    public float pitchIncrement = 0.5f;   // Amount by which to increase the pitch each time
    public float maxPitch = 3.0f;         // Maximum pitch value to prevent it from increasing indefinitely

    private float initialPitch = 1.0f;           // Store the initial pitch to reset

    bool tileOccupied = false;
    
    void Start()
    {
        //occupy tiles so that a cannon or a modifier cannot be placed there
        Debug.Log("instamce" + GameManager.instance == null);
        Debug.Log("tilemap" + GameManager.instance.getTilemap() == null);
        if (GameManager.instance.getTilemap() != null)
        {
            GameManager.instance.getTilemap().occupyTile(transform.position);
            tileOccupied = true;
        }
        changeText(HPInt.ToString());
        
        Debug.Log("Finding healthbar slider...");
        // Find the Slider component in the children of the current GameObject
        healthBar = GetComponentInChildren<Slider>();
        // If the Slider is found, set its maxValue
        if (healthBar != null)
        {
            healthBar.maxValue = HPInt;
            healthBar.value = HPInt; // Initialize the slider with current health value
        }
        else
        {
            Debug.LogError("Slider component not found in children!");
        }
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!tileOccupied)
        {
            GameManager.instance.getTilemap().occupyTile(transform.position);
            tileOccupied = true;
        }

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, Vector2.zero, 0f, enemyMask);
        
        if (hits.Length > 0)
        {
            Debug.Log("Detected Enemy in rum");
            int regularEnemies = 0;
            int krakenEnemies = 0;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    var kraken = hit.collider.GetComponent<Kraken>();
                    if (kraken != null && !kraken.isEnemyDying())
                    {
                        krakenEnemies++;
                    }
                    var enemy = hit.collider.GetComponent<EnemyController>();
                    if (enemy != null && !enemy.isEnemyDying())
                    {
                        // Check if the enemy is a Mermaid and if it is in stealth
                        var mermaid = enemy.GetComponent<Mermaid>();
                        if (mermaid == null || !mermaid.isInStealth())
                        {
                            regularEnemies++;
                        }
                    }
                }
            }

            damageTimer += Time.deltaTime;

            // Take damage if a second has passed
            if (damageTimer >= 1f)
            {
                Debug.Log("Tapping rum");
                TakeDamage(regularEnemies, krakenEnemies);
                damageTimer = 0f;
            }
        }
        else
        {
            audioSource.pitch = initialPitch;
        }
    }

    void TakeDamage(int enemiesNum, int krakenEnemies)
    {
        // Each regular enemy causes 1 damage per second
        int totalDamage = enemiesNum;

        // Each kraken enemy causes additional damage based on the multiplier
        totalDamage += Mathf.CeilToInt(krakenEnemies * krakenMultiplier);

        // Apply the damage
        HPInt -= totalDamage;

        // Play damage sound and update pitch
        if (totalDamage > 0)
        {
            audioSource.PlayOneShot(onDamageSound);
            audioSource.pitch = Mathf.Min(audioSource.pitch + pitchIncrement, maxPitch);
        }

        // Update UI
        changeText(HPInt.ToString());
        healthBar.value = HPInt;

        // Check for game over
        if (HPInt <= 0)
        {
            audioSource.pitch = initialPitch;
            audioSource.PlayOneShot(onLossSound);
            GameManager.instance.GameLost();
        }
    }

   
    public void changeText(string newText)
    {
        TextMeshProUGUI textComponent = canvas.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = newText;
    }
}
