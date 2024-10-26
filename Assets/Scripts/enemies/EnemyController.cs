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
    private float delay = 1f;
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

            //makes sure that the cannon ball is deactivated before destroying an enemy object
            bool finised = other.GetComponent<CannonBallController>().deactivate();
            if(finised)
                TakeDamage(damage);
        }
    }

    public virtual void TakeDamage(int dmg)
    {
        audioSource.PlayOneShot(onHitSound, audioSource.volume);
        health -= dmg;
        health = health < 0 ? 0 : health;
        changeText(health.ToString());
        Debug.Log(health);
        if (health <= 0) {
            StartCoroutine(Die());
        }
    }
    
    public virtual IEnumerator Die()
    {
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
