using BarthaSzabolcs.Tutorial_SpriteFlash;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using static UnityEngine.InputManagerEntry;

public class EnemyController : MonoBehaviour
{
    protected int health;
    protected float speed;
    protected GameObject[] waypoints;
    protected int currentWaypoint = 0;
    protected Vector2 finalDestination;
    protected Vector2 currentDestination;
    public Canvas canvas;
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    protected AudioSource audioSource;
    public AudioMixerGroup sfxMixerGroup;
    [SerializeField] protected AudioClip onHitSound;
    [SerializeField] protected AudioClip onDeathSound;
    [SerializeField]  protected ParticleSystem damageParticles;
    private float delay = 1f;
    private int priceForKill = 20;

    // add generic speed
    
    //add generic base_HP

    private void Start()
    {
      
        
    }

  
    protected virtual void Prepare()
    {
        waypoints = EnemyPathManager.Instance.getRandomPath();
        currentDestination = waypoints[currentWaypoint].GetComponentInParent<Transform>().position;
    

        finalDestination = GameManager.instance.getRumPosition();
        changeText(health.ToString());
        
    }
    protected virtual IEnumerator SlowDown(float newSpeed)
    {
        //float originalSpeed = 1.5f;
        speed = newSpeed;  // Apply slowing effect factor to speed
        //Debug.Log("Enemy speed: " + speed);
        yield return new WaitForSeconds(1f);  // Slow effect lasts for 3 seconds

        // Restore original speed after effect ends
        //speed = originalSpeed;
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
            

            //makes sure that the cannon ball is deactivated before destroying an enemy object
            bool finised = other.GetComponent<CannonBallController>().deactivate();
            if (finised)
            {
                TakeDamage(damage);
                StartCoroutine(SlowDown(newSpeed));
            }

        }
    }

    public virtual void TakeDamage(int dmg)
    {
        


        health -= dmg;
        health = health < 0 ? 0 : health;
        changeText(health.ToString());
        Instantiate(damageParticles, this.transform.position, Quaternion.identity);

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
                StartCoroutine(FlashColor(Color.red, 0.1f)); // Flash to red for 0.1 seconds uncomment if you want to try this one out and comment the other one
                //StartCoroutine(FadeEffect(0.3f));
            }
        }
        
        
    }


    public virtual IEnumerator Die()
    {
        //Slow down
        StartCoroutine(SlowDown(0f));
        
        //Fade out
        StartCoroutine(FadeEffect(0.5f));
        
        //Drop gold
        MarketManager.instance.earnGold(priceForKill);
        if(onDeathSound != null)
            audioSource.PlayOneShot(onDeathSound, audioSource.volume);
        
        
        Debug.Log("Enemy has died.");
        yield return new WaitForSeconds(delay);
        Destroy(gameObject); // Remove enemy from the scene
       

    }

    // Flash the SpriteRenderer color
    private IEnumerator FlashColor(Color flashColor, float duration)
    {
        Color originalColor = spriteRenderer.color;

        
        spriteRenderer.color = flashColor;

      
        yield return new WaitForSeconds(duration);

        // Revert to the original color
        spriteRenderer.color = originalColor;
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
}
