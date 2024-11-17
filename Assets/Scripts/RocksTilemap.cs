using UnityEngine;

public class RocksTilemap : MonoBehaviour
{
    [SerializeField] private AudioClip onHit;
    private AudioSource audioSource;
    [SerializeField] private ParticleSystem onHitParticles;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void initParticles(Vector3 ballPosition) {
        Quaternion rotation = Quaternion.Euler(0, 0, 90);
        Vector3 finalPosition = new Vector3(ballPosition.x + 0.5f, ballPosition.y, ballPosition.z);
        
        Instantiate(onHitParticles, finalPosition, rotation);
    }
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
        {
            var cannonBallControllerComponent = collision.GetComponent<CannonBallController>();

            initParticles(cannonBallControllerComponent.transform.position);
            
            
            audioSource.PlayOneShot(onHit, audioSource.volume);


            cannonBallControllerComponent.deactivate();
        }
    }
}