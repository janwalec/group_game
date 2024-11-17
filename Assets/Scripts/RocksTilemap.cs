using UnityEngine;

public class RocksTilemap : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
        {
            collision.GetComponent<CannonBallController>().deactivate();
        }
    }
}