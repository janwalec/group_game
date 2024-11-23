using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageSequenceController : MonoBehaviour
{
    public Image[] images;  // Assign the images in the inspector
    public AnimationCurve fadeCurve;
    public AnimationCurve scaleCurve;
    public float fadeDuration;
    private int currentImageIndex = -1;
    
    public AudioClip[] audioClips;
    private AudioSource audioSource;  // Reference to the AudioSource component

    void Start()
    {
        // Ensure all images start hidden and at the desired scale
        foreach (Image img in images)
        {
            img.color = new Color(0, 0, 0, 0);
            img.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(StartAfterDelay());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentImageIndex > images.Length - 1)
            {
                MainMenuGameManager.instance.Play();
                return;
            }

            ShowNextImage();
        }
    }

    private IEnumerator StartAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(1.0f);
        
        // Show the first image
        ShowNextImage();
    }

    void ShowNextImage()
    {
        
        if (currentImageIndex >= 0)
        {
            // Start the fade-out coroutine for the current image
            StartCoroutine(FadeOutImage(images[currentImageIndex]));
        }

        currentImageIndex++;

        if (currentImageIndex < images.Length)
        {
            // Start the coroutine to show the next image
            StartCoroutine(FadeInImage(images[currentImageIndex]));

            // Play the corresponding audio clip
            if (audioSource != null && audioClips[currentImageIndex] != null)
            {
                audioSource.clip = audioClips[currentImageIndex];
                audioSource.Play();
            }
        }
     
    }

    IEnumerator FadeOutImage(Image img)
    {
        float duration = 0.7f; // duration of the fade
        float currentTime = 0f;

        while (currentTime < duration)
        {
            float t = currentTime / duration;
            float alpha = 1 - fadeCurve.Evaluate(t); // Invert the alpha for fading out
            img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }

        img.color = new Color(img.color.r, img.color.g, img.color.b, 0); // Ensure it's fully transparent
    }
    
    IEnumerator FadeInImage(Image img)
    {
        float duration = fadeDuration; // duration of the fade
        float currentTime = 0f;

        while (currentTime < duration)
        {
            float t = currentTime / duration;
            float alpha = fadeCurve.Evaluate(t);
            float scale = Mathf.Lerp(1.2f, 1.0f, scaleCurve.Evaluate(t));
            img.color = new Color(1, 1, 1, alpha);
            img.transform.localScale = new Vector3(scale, scale, scale);
            currentTime += Time.deltaTime;
            yield return null;
        }

        img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        img.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
