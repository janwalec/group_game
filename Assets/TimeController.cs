using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeController : MonoBehaviour
{
    public UIDocument uiDocument; // Reference to the UIDocument component
    private Button fastForwardButton;
    public float normalSpeed = 1f;
    public float fastForwardSpeed = 2f;
    public bool isFastForwarding = false;

    public static TimeController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            
            Time.timeScale = normalSpeed;
        }
    }
    

    public void SetNormalSpeed()
    {
        // Reset time scale
        isFastForwarding = false;
        Time.timeScale = normalSpeed;
    }
    
    public void ToggleFastForward()
    {
        Debug.Log("ff.Toggled fast-forward");
        if (isFastForwarding)
        {
            Time.timeScale = normalSpeed;
            Debug.Log("ff.off");
        }
        else
        {
            Time.timeScale = fastForwardSpeed;
            Debug.Log("ff.on");
        }

        isFastForwarding = !isFastForwarding;
    }

    void OnDestroy()
    {
        Time.timeScale = normalSpeed;
    }
}
