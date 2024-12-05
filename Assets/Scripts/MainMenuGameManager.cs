using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuGameManager : MonoBehaviour
{
    public static MainMenuGameManager instance;
    [SerializeField] private GameObject settingsUI;
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

    public void Settings()
    {
        settingsUI.SetActive(!settingsUI.activeSelf);
    }

}