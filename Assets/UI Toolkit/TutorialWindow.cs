using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.Video;

public class TutorialWindow : MonoBehaviour
{
    // Define a TutorialStep class to hold data for each step
    [Serializable] public class TutorialStep
    {
        public string text;
        public Sprite image; // You can also use Texture2D or any other type
        public VideoClip video; // Optional if you want to use videos
    }

    [SerializeField] public List<TutorialStep> tutorialSteps;
    [SerializeField] private VideoPlayer videoPlayer;
    private int currentStep = 0;

    private Label tutorialText;
    private VisualElement tutorialImageContainer;
    private Image tutorialImage;
    private Button previousButton;
    private Button nextButton;
    private Button closeButton;
    
    private RenderTexture renderTexture;
    private Texture2D videoTexture;  // This will store the converted Texture2D
    
    private VisualElement root;

    private bool isTutorialActive = false;
    
    private void OnEnable()
    {
        // Check if it's the first time opening the game
        if (PlayerPrefs.GetInt("FirstTime", 1) == 1)
        {
            // First time, show tutorial
            isTutorialActive = true;
            PlayerPrefs.SetInt("FirstTime", 0); // Mark that the tutorial has been shown
            PlayerPrefs.Save(); // Save the PlayerPrefs immediately
        }
        else
        {
            isTutorialActive = false;
        }
        
        root = GetComponent<UIDocument>().rootVisualElement;

        tutorialText = root.Q<Label>("TutorialText");
        tutorialImageContainer = root.Q<VisualElement>("TutorialImage");
        previousButton = root.Q<Button>("PreviousButton");
        nextButton = root.Q<Button>("NextButton");

        previousButton.clicked += OnPreviousButtonClick;
        nextButton.clicked += OnNextButtonClick;
        
        if (tutorialImageContainer != null)
        {
            tutorialImage = new Image();
            tutorialImageContainer.Add(tutorialImage);
        }
        else
        {
            Debug.LogError("tutorialImageContainer is null. Image cannot be added.");
        }
        
        // Initialize VideoPlayer and RenderTexture
        //videoPlayer = GetComponent<VideoPlayer>();
        renderTexture = new RenderTexture(404, 300, 24);
        videoPlayer.targetTexture = renderTexture;

        // Initialize Texture2D for converting RenderTexture
        videoTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        // Set the RenderTexture to a VisualElement to show the video
        var videoImage = new Image();
        tutorialImageContainer.Add(videoImage);
        
        
        //Setup xclose button
        closeButton = root.Q<Button>("XClose");
        if (closeButton != null)
        {
            closeButton.RegisterCallback<ClickEvent>(ev => OnCloseShopButtonClick());
        }
        
        // Update the tutorial step only if the tutorial is active
        if (isTutorialActive)
        {
            UpdateTutorialStep();
        }
        else
        {
            root.style.display = DisplayStyle.None; // Hide tutorial window if not first time
        }
    }
    
    public void ToggleTutorialWindow()
    {
        if (root.style.display == DisplayStyle.Flex)
        {
            root.style.display = DisplayStyle.None; // Hide tutorial window
        }
        else
        {
            root.style.display = DisplayStyle.Flex; // Show tutorial window
            UpdateTutorialStep(); // Refresh the tutorial content
        }
    }

    private void OnPreviousButtonClick()
    {
        if (currentStep > 0)
        {
            currentStep--;
            UpdateTutorialStep();
        }
    }

    private void OnNextButtonClick()
    {
        if (currentStep < tutorialSteps.Count - 1)
        {
            currentStep++;
            UpdateTutorialStep();
        }
        else
        {
            OnCloseShopButtonClick();
        }
    }

    private void UpdateTutorialStep()
    {
        tutorialImageContainer.Clear();
        var step = tutorialSteps[currentStep];
        tutorialText.text = step.text;
        
        if (step.image != null) //IMAGE
        {
            tutorialImage = new Image();
            tutorialImageContainer.Add(tutorialImage);
            tutorialImage.sprite = step.image;
            videoPlayer.Stop();
            var videoImage = tutorialImageContainer.Query<VisualElement>().First();
            videoImage.style.backgroundImage = null;
        }
        else if (step.video != null) //VIDEO
        {
            var videoImage = new Image();
            tutorialImageContainer.Add(videoImage);
            Debug.Log("Trying to play video");
            videoPlayer.clip = step.video;
            videoPlayer.Play();
        }
        else
        {
            tutorialImageContainer.Clear();
            videoPlayer.Stop();
        }
        // Convert RenderTexture to Texture2D
        if (videoPlayer.isPlaying)
        {
            RenderTexture.active = renderTexture;
            videoTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            videoTexture.Apply();
            RenderTexture.active = null;

            var videoImage = tutorialImageContainer.Query<VisualElement>().First();
            videoImage.style.backgroundImage = new StyleBackground(videoTexture);
        }

        previousButton.SetEnabled(currentStep > 0);
        nextButton.SetEnabled(currentStep < tutorialSteps.Count - 1);
    }

    private void OnCloseShopButtonClick()
    {
        root.style.display = DisplayStyle.None;
        //transform.localScale = Vector3.zero;
    }
    
    private void Update()
    {
        // Continuously update the video frame if the video is playing
        if (videoPlayer.isPlaying)
        {
            UpdateVideoFrame();
        }
    }
    private void UpdateVideoFrame()
    {
        // Convert RenderTexture to Texture2D
        RenderTexture.active = renderTexture;
        videoTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        videoTexture.Apply();
        RenderTexture.active = null;

        // Update the video image in the UI
        var videoImage = tutorialImageContainer.Query<VisualElement>().First();
        videoImage.style.backgroundImage = new StyleBackground(videoTexture);
    }
}
