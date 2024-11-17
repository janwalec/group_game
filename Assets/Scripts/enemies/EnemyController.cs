using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    protected int health;
    protected float speed;
    protected GameObject[] waypoints;
    protected int currentWaypoint = 0;
    protected Vector2 finalDestination;
    protected Vector2 currentDestination;
    public Canvas canvas;
    protected AudioSource audioSource;
    [SerializeField] protected AudioClip onHitSound;
    [SerializeField] protected AudioClip onDeathSound;

    [SerializeField]  protected ParticleSystem damageParticles;
    private float delay = 1f;
    private int priceForKill = 20;

    // add generic speed

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
        if(onHitSound != null)
            audioSource.PlayOneShot(onHitSound, audioSource.volume);
        health -= dmg;
        health = health < 0 ? 0 : health;
        changeText(health.ToString());
        Instantiate(damageParticles, this.transform.position, Quaternion.identity);
        if (health <= 0) {
            StartCoroutine(Die());
        }
    }
    
    public virtual IEnumerator Die()
    {

        MarketManager.instance.earnGold(priceForKill);
        if(onDeathSound != null)
            audioSource.PlayOneShot(onDeathSound, audioSource.volume);
        Debug.Log("Enemy has died.");
        yield return new WaitForSeconds(delay);
        Destroy(gameObject); // Remove enemy from the scene
       

    }

    public void changeText(string newText)
    {
        TextMeshProUGUI textComponent = canvas.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = newText;
    }
}
