using System;
using UnityEditor.Tilemaps;
using UnityEngine;

public class CannonBallController : MonoBehaviour
{

    private float speed = 8f;
    private int range = 50;
    
    private Transform enemyToFollow;
    private int damage = 1;
    private float timer;
    public LayerMask enemyMask;
    void Start()
    {

    }

    void Update()
    {
        if (GameManager.instance.currentGameState != GameState.GS_BATTLE)
        {
            return;
        }
        Move();
        
     
    }

    private void Move()
    {
        if (enemyToFollow == null || enemyToFollow.gameObject.active == false)
        {
            FindNewEnemy();
        }
        else
        {
            //moves in the direction specified by the direction variable
            transform.position = Vector2.MoveTowards(transform.position, enemyToFollow.position, speed * Time.deltaTime);
            Debug.Log("to " + enemyToFollow.position);
        }
        if (transform.position.magnitude > range)
        {
            gameObject.SetActive(false);
        }
    }

    private void FindNewEnemy()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            enemyToFollow = hits[0].transform;
            return;
        }
        else
        {
            deactivate();
        }
    }

    public void setDirection(Transform enemy)
    {
        enemyToFollow = enemy;
    }
    public bool deactivate()
    {
        gameObject.SetActive(false);
        return true;
    }
    public void setDamage(int damage_)
    {
        damage = damage_;
    }
    public int getDamage()
    {
        return damage;
    }

}