using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLostManager : MonoBehaviour
{
    public static GameLostManager instance;
    public void Awake()
    {
        instance = this;
    }
    public void Play()
    {
        SceneManager.LoadScene("Scene");
    }
}