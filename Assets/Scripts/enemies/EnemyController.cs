using System;
using BarthaSzabolcs.Tutorial_SpriteFlash;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class EnemyController : MonoBehaviour
{
    protected int health;
    protected float speed;
    protected float normalSpeed;
    public GameObject[] waypoints;
    public int currentWaypoint = 0;
    protected Vector2 finalDestination;
    protected Vector2 currentDestination;
    public Canvas canvas;
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    protected AudioSource audioSource;
    public AudioMixerGroup sfxMixerGroup;
    [SerializeField] protected AudioClip onHitSound;
    [SerializeField] protected AudioClip onDeathSound;
    [SerializeField] protected AudioClip dropGold;

    [SerializeField] protected ParticleSystem damageParticles;
    //[SerializeField] protected float speedMultiplier = 1.0f;
    //[SerializeField] protected int healthAddition = 10;
    private float delay = 2f;
    public int priceForKill;
    private bool isDying = false;
    private bool isTakingDamage = false;
    private int lastDamageTaken = 0;
    private bool lastHitHasGoldMultiplier;
    public string shipType = "Generic Enemy";
    protected Slider healthBar;

    protected void ApplyHealthAddition()
    {
        priceForKill = health;
        // Ensure EnemyManager exists
        if (EnemyManager.Instance != null)
        {
            int additionalHealth = EnemyManager.Instance.HealthAddition;
            health += additionalHealth; // Add the global health addition to the base health
            Debug.Log($"{name} Final Health: {health}");
        }
        else
        {
            Debug.LogWarning("EnemyManager is not present in the scene.");
        }
    }

    protected void ApplySpeedMultiplication()
    {
        // Ensure EnemyManager exists
        if (EnemyManager.Instance != null)
        {
            float additionalSpeed = EnemyManager.Instance.SpeedMultiplication;
            speed *= additionalSpeed; // Add the global speed gain
            Debug.Log($"{name} Final speed: {speed}");
            
            //Save its normalspeed
            normalSpeed = speed;
        }
        else
        {
            Debug.LogWarning("EnemyManager is not present in the scene.");
        }
    }


    private void OnValidate()
    {
        
        //this.speed = this.speed * speedMultiplier;
        //Debug.Log("Speed after adjusting in game is:" + this.speed);
        //this.health = this.health + healthAddition;
        //Debug.Log("Health after adjusting in game is:" + this.health);

    }

    private void Start()
    {
        
    }

 

    protected virtual void Prepare()
    {
        waypoints = EnemyPathManager.Instance.getRandomPath();
        currentDestination = waypoints[currentWaypoint].GetComponentInParent<Transform>().position;
    

        finalDestination = GameManager.instance.getRumPosition();
        changeText(health.ToString());
        
        
        // If the Slider is found, set its maxValue
        if (healthBar != null)
        {
            healthBar.maxValue = health;
            healthBar.value = health; // Initialize the slider with current health value
        }
        else
        {
            Debug.LogError("Slider component not found in children!");
        }

    }
    protected virtual IEnumerator SlowDown(float newSpeed)
    {
        float cappedNewSpeed = newSpeed;
        if (newSpeed > 1f)
        {
            cappedNewSpeed = 1f;
        }
        speed *= cappedNewSpeed; 
        //Debug.Log("Enemy speed: " + speed);
        yield return new WaitForSeconds(2f);
        speed = normalSpeed;
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // Assign the Audio Mixer Group to the AudioSource
        if (audioSource != null && sfxMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found in Enemy or its children!");
        }
        
        Debug.Log("Finding healthbar slider...");
        // Find the Slider component in the children of the current GameObject
        healthBar = GetComponentInChildren<Slider>();

    }
    private void Update()
    {
        //Move();
   
    }
    private void FixedUpdate()
    {
        Move();
    }
    public virtual void Move()
    {

        if(GameManager.instance.currentGameState != GameState.GS_BATTLE)  
        {
            return;
        }
        //moves towards waypoints specified in the waypoints[] array
      

        if (Vector2.Distance(this.transform.position, waypoints[currentWaypoint].transform.position) < 0.1f)
        {
            if (currentWaypoint == waypoints.Length - 1)
            {
                currentDestination = finalDestination;
            }
            else
            {
                currentWaypoint = (currentWaypoint + 1 % waypoints.Length);
                currentDestination = waypoints[currentWaypoint].GetComponentInParent<Transform>().position;
                Debug.Log($"{shipType} Next Waypoint Destination: " + currentDestination); // Include shipType in log
            }
        }
        //transform.position = Vector2.MoveTowards(transform.position, currentDestination, speed * Time.deltaTime);
        GetComponent<Rigidbody2D>().MovePosition(Vector2.MoveTowards(transform.position, currentDestination, speed * Time.deltaTime));
        //transform.position = Vector2.MoveTowards(transform.position, currentDestination, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //takes damage whenever hit by a bullet
        if (other.CompareTag("Bullet"))
        {
            int damage = other.GetComponent<CannonBallController>().getDamage();
            float newSpeed = other.GetComponent<CannonBallController>().getSlowingEffect();
            bool isBouncy = other.GetComponent<CannonBallController>().IsBouncy();
            Debug.Log("The cb wasBouncy: " + isBouncy);
            bool finised = false;
            CannonBallController cb = other.GetComponent<CannonBallController>();
            lastDamageTaken = damage;
            lastHitHasGoldMultiplier = cb.GetHasGoldMultiplier();
            if (isBouncy)
            {
                cb.FindNewEnemyBounce(this.gameObject);
                finised = true;
            }
            else
            {
                finised = cb.deactivate();
            }

            //makes sure that the cannon ball is deactivated before destroying an enemy object
            //finised = other.GetComponent<CannonBallController>().deactivate();
            if (finised)
            {
                TakeDamage(damage);
                StartCoroutine(SlowDown(newSpeed));
            }

        }
    }

    public virtual void TakeDamage(int dmg)
    {

        Debug.Log($"{name} took damage: {dmg}, current health: {health}");
        health -= dmg;
        health = health < 0 ? 0 : health;
        if(!isDying) {
            changeText(health.ToString());
            healthBar.value = health;
        }
        Instantiate(damageParticles, this.transform.position, Quaternion.identity);

        print("HP is" + health);

        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            if (onHitSound != null)
                audioSource.PlayOneShot(onHitSound, audioSource.volume);


            if (spriteRenderer != null)
            {
                StartCoroutine(FlashColor(Color.red, 0.05f)); // Flash to red for 0.1 seconds uncomment if you want to try this one out and comment the other one
                //StartCoroutine(FadeEffect(0.3f));
            }
        }
        
    }


    public virtual IEnumerator Die()
    {
        
        if (isDying) yield break; //If it already is dying, just do nothing.
        isDying = true;

        healthBar.gameObject.SetActive(false);
    
        //Calculate how much gold should be dropped. Overwritten if there is a goldMultiplier.
        int goldToDrop = priceForKill;
        if (lastHitHasGoldMultiplier)
        {
            int bonus = Mathf.RoundToInt(lastDamageTaken * EnemyManager.Instance.GoldBonusFactor);
            goldToDrop = bonus + priceForKill;
            //Wait for 0.8 seconds, then call ShowGoldText with (bonus)
            StartCoroutine(DelayedShowGoldText(bonus, 0.5f));
        }
        
        ShowGoldText(priceForKill);
        
        //Slow down
        StartCoroutine(SlowDown(0f));
        
        //Fade out
        StartCoroutine(FadeEffect(0.5f));
        
        //Drop gold
        if (MarketManager.instance != null)
        {
            MarketManager.instance.earnGold(goldToDrop);
        }
        if(onDeathSound != null)
            audioSource.PlayOneShot(onDeathSound, audioSource.volume);
        
        // Notify subscribers that this enemy has been killed
        GameEvents.EnemyKilled(goldToDrop);
        
        Debug.Log("Enemy has died.");
        yield return new WaitForSeconds(delay);
        Destroy(gameObject); // Remove enemy from the scene
       

    }

    // Flash the SpriteRenderer color
    private IEnumerator FlashColor(Color flashColor, float duration)
    {
        if (isTakingDamage) yield break; //If it already is dying, just do nothing.
        isTakingDamage = true;
        Color originalColor = spriteRenderer.color;

        
        spriteRenderer.color = flashColor;

      
        yield return new WaitForSeconds(duration);

        // Revert to the original color
        spriteRenderer.color = originalColor;
        isTakingDamage = false;
    }


    // flash version 2, just to fade in fade out quickly
    private IEnumerator FadeEffect(float duration)
    {
        // Get the original color of the sprite
        Color originalColor = spriteRenderer.color;
    
        // Set the starting alpha value to 1 (fully opaque)
        float startAlpha = 1.0f;
        // Set the ending alpha value to 0 (fully transparent)
        float endAlpha = 0.0f;

        // Store the current time
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            // Calculate the elapsed time as a fraction of the total duration
            float elapsed = (Time.time - startTime) / duration;
        
            // Interpolate the alpha value based on the elapsed time
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed);
        
            // Update the sprite's color with the new alpha value
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
        
            // Wait until the next frame before continuing the loop
            yield return null;
        }

        // Ensure the final alpha value is set to 0 (completely transparent)
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }


    public void changeText(string newText)
    {
        TextMeshProUGUI textComponent = canvas.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = newText;
    }
    
    private void ShowGoldText(int goldDropped)
    {
        TextMeshProUGUI textComponent = canvas.GetComponentInChildren<TextMeshProUGUI>();
        //textComponent.gameObject.SetActive(true);
        changeText("+" + goldDropped);
        textComponent.color = Color.yellow;
        
        StartCoroutine(MoveAndFadeText(textComponent));
    }
    
    private IEnumerator DelayedShowGoldText(int bonus, float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowGoldText(bonus);
    }
    
    private IEnumerator MoveAndFadeText(TextMeshProUGUI textComponent)
    {
        
        float moveDuration = 0.5f;

        Vector2 originalPosition = textComponent.transform.position;
        Color originalColor = textComponent.color;
        float elapsedTime = 0f;

        // Move up
        while (elapsedTime < moveDuration)
        {
            textComponent.transform.position = originalPosition + Vector2.up * 0.2f * (elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0.5f, elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        audioSource.PlayOneShot(dropGold);
        textComponent.alpha = 0f;
    }

    public bool isEnemyDying()
    {
        return isDying;
    }

}
