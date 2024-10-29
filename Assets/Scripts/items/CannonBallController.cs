using System;
using UnityEditor.Tilemaps;
using UnityEngine;

public class CannonBallController : MonoBehaviour
{

    private float speed = 8f;
    private int range = 50;
    private Vector3 direction = new Vector3(1, 0, 0);
    private int damage = 1;
    private float timer;
    void Start()
    {

    }

    void Update()
    {
        if (GameManager.instance.currentGameState != GameState.GS_GAME)
        {
            return;
        }
        //moves in the direction specified by the direction variable
        transform.position = Vector2.MoveTowards(transform.position, direction, speed * Time.deltaTime);

        if (transform.position.magnitude > range)
        {
            gameObject.SetActive(false);
        }


        if (Vector2.Distance(transform.position, direction) < 1f)
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                deactivate();
                timer = 0f;
            }

        }
    }

    public void setDirection(Vector3 newDirection)
    {
        direction = newDirection;
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