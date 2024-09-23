using System;
using UnityEditor.Tilemaps;
using UnityEngine;

public class CannonBallController : MonoBehaviour
{

    private float speed = 0.1f;
    private int range = 50;
    private Vector3 direction = new Vector3(1, 0, 0);
    void Start()
    {

    }

    void Update()
    {
        float ratio = direction.x / direction.y;
        double y = Math.Pow(Math.Pow(speed, 2.0) / (1 + Math.Pow(ratio, 2.0)), 0.5);
        double x = Math.Pow(Math.Pow(speed, 2.0) - Math.Pow(y, 2.0), 0.5);
        x = direction.x > 0 ? -x : x;
        y = direction.y > 0 ? -y : y;
        Vector3 newPosition = new Vector3(this.transform.position.x + (float)x, this.transform.position.y + (float)y, this.transform.position.z);
        this.transform.position = newPosition;
        if(transform.position.magnitude > range)
        {
            gameObject.SetActive(false);
        }
        //Debug.Log(x + " " + y + " " + (x * x + y * y));
    }

    public void setDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }
}