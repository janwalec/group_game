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
    void Start()
    {
        //occupy tiles so that a cannon or a modifier cannot be placed there
        GameManager.instance.getTilemap().occupyTile(transform.position);
        changeText(HPInt.ToString());
    }


    void Update()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            TakeDamage(hits.Length);
        }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void TakeDamage(int enemiesNum)
    {
        HPLoss += enemiesNum * 0.01;
        if(HPLoss >= 1)
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
