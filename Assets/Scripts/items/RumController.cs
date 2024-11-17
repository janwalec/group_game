using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    private double krakenMultiplier = 2.0;  // Kraken consumes rum at twice the normal rate
    
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
            int regularEnemies = 0;
            int krakenEnemies = 0;

          
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    
                    if (hit.collider.GetComponent<Kraken>() != null)
                    {
                        krakenEnemies++;
                    }
                    else
                    {
                        regularEnemies++;
                    }
                }
            }


            TakeDamage(regularEnemies, krakenEnemies);
        }
        else
        {
            audioSource.pitch = initialPitch;
        }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void TakeDamage(int enemiesNum, int krakenEnemies)
    {
        HPLoss += enemiesNum * 0.01;

        // Calculate additional HPLoss for Kraken enemies with a higher rate

        HPLoss += krakenEnemies * 0.01 * krakenMultiplier;


        if (HPLoss >= 1)
        {
            audioSource.PlayOneShot(onDamageSound);
            audioSource.pitch = Mathf.Min(audioSource.pitch + pitchIncrement, maxPitch); // Increase pitch but do not exceed maxPitch
            audioSource.PlayOneShot(onDamageSound);
            HPInt--;
            HPLoss = 0;
            changeText(HPInt.ToString());
        }
        
        if(HPInt <= 0)
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
