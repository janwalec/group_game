using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    protected int health;
    protected float speed; // maybe not needed? 
    [SerializeField] protected GameObject[] waypoints;
    protected int currentWaypoint = 0;
    protected Vector2 finalDestination;
    protected Vector2 currentDestination;
    protected int bulletDamage = 1;

    private void Start()
    {
        //sets the starting destination on the closest waypoint and the final destination as the rum
        currentDestination = waypoints[currentWaypoint].GetComponentInParent<Transform>().position;
        finalDestination = GameManager.instance.getRumPosition();
    }

    private void Update()
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
        transform.position = Vector2.MoveTowards(transform.position, currentDestination, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //takes damage whenever hit by a bullet
        if (other.CompareTag("Bullet"))
        {
            //makes sure that the cannon ball is deactivated before destroying an enemy object
            bool finised = other.GetComponent<CannonBallController>().deactivate();
            if(finised)
                TakeDamage(bulletDamage);
        }
    }

    public virtual void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log(health);
        if (health <= 0) {
            Die();
        }
    }
    
    public virtual void Die()
    {
        Debug.Log("Enemy has died.");
        Destroy(gameObject); // Remove enemy from the scene
    }
}
