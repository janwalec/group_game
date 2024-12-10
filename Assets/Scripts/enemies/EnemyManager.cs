using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [SerializeField] private int healthAddition = 10;
    [SerializeField] private float speedMultiplication = 1.0f; // default values 
    [SerializeField] private float goldBonusFactor = 0.5f;

    
    public int HealthAddition => healthAddition;
    public float SpeedMultiplication => speedMultiplication;
    
    public float GoldBonusFactor
    {
        get => goldBonusFactor;
        set => goldBonusFactor = value;
    }

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

    public void SetHealthAddition(int health)
    {
        healthAddition = health;
    }

    public int getHealthAddition()
    {
        return healthAddition;
    }
}
