using UnityEditor.Tilemaps;
using UnityEngine;

public class CannonBallController : MonoBehaviour
{

    private float speed = 0.1f;
    private int range = 50;
    void Start()
    {

    }

    void Update()
    {
        this.transform.position += new Vector3(speed, 0, 0);
        if(transform.position.magnitude > range)
        {
            gameObject.SetActive(false);
        }
        
    }
}