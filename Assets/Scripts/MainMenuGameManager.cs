using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuGameManager : MonoBehaviour
{
    public static MainMenuGameManager instance;
    public void Awake()
    {
        instance = this;
    }
    public void Play()
    {
        SceneManager.LoadScene("Scene");
    }
    public void Intro()
    {
        Debug.Log("Intro");
        SceneManager.LoadScene("IntroScene");
    }
}