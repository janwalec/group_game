using System;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class CannonBallController : MonoBehaviour
{

    private float speed = 8f;
    private int range = 50;
    private float newSlowedSpeed = 1.0f;
    private Transform enemyToFollow;
    private int damage = 1;
    private bool isBouncy;
    private bool hasBounced = false;
    private bool hasGoldMultiplier;
    private float timer;
    public LayerMask enemyMask;
    void Start()
    {
        hasBounced = false;
    }

    void Update()
    {
        if (GameManager.instance.currentGameState != GameState.GS_PAUSEMENU)
        {
            if (GameManager.instance.currentGameState != GameState.GS_BATTLE)
            {
                deactivate();

            }

            Move();
        }
     
    }

    public void setSlowingEffect(float newSpeed)
    {
        this.newSlowedSpeed = newSpeed;
    }

    public float getSlowingEffect()
    {
        return this.newSlowedSpeed;
    }

    public void setBouncy(bool willBounce)
    {
        this.isBouncy = willBounce;
        Debug.Log("The cb wasBouncy: " + isBouncy);
    }

    public bool IsBouncy()
    {
        return isBouncy;
    }

    public void SetHasGoldMultiplier(bool setHasGoldMultiplier)
    {
        hasGoldMultiplier = setHasGoldMultiplier;
    }

    public bool GetHasGoldMultiplier()
    {
        return hasGoldMultiplier;
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
    
    public void FindNewEnemyBounce(GameObject currentTarget)
    {
        Debug.Log("FindNewEnemyBounce was called. hasBounced:" + hasBounced.ToString());
        if (hasBounced)
        {
            Debug.Log("HasBounced already");
            deactivate();
            return;
        }
        range = 50;
        damage = damage / 3;
        if (damage < 1)
        {
            deactivate();
            return;
        }
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, transform.position, 0f, enemyMask);
        
        if (hits.Length > 1)
        {
            int i = 0;
            // Loop through hits until we find a target that is not the current target
            while (i < hits.Length && hits[i].transform == currentTarget.transform)
            {
                i++;
                Debug.Log("Looking for new target... " + i.ToString());
            }
            // Check if we found a valid target
            if (i < hits.Length)
            {
                enemyToFollow = hits[i].transform;
                hasBounced = true;
            }
            return;
        }
        else
        {
            Debug.Log("No valid new target found");
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
        hasBounced = false; //Reset hasBounced.
        isBouncy = false; //Reset isBouncy
        hasGoldMultiplier = false; //Reset multiplier
        newSlowedSpeed = 1.0f; //Reset slow
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