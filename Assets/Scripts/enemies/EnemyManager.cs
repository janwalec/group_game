using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [SerializeField] private int healthAddition = 10;
    [SerializeField] private float speedMultiplication = 1.0f; // default values 

    public int HealthAddition => healthAddition;
    public float SpeedMultiplication => speedMultiplication;

    private void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

       
        DontDestroyOnLoad(gameObject);
    }
}
