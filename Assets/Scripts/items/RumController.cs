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
    private double krakenMultiplier = 2.0;  // Kraken consumes rum at twice the normal rate
    void Start()
    {
        //occupy tiles so that a cannon or a modifier cannot be placed there
        GameManager.instance.getTilemap().occupyTile(transform.position);
        changeText(HPInt.ToString());
    }


    void Update()
    {
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
            HPInt--;
            HPLoss = 0;
            changeText(HPInt.ToString());
        }
        
        if(HPInt <= 0)
        {
            GameManager.instance.GameLost();
        }
    }

   
    public void changeText(string newText)
    {
        TextMeshProUGUI textComponent = canvas.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = newText;
    }
}
