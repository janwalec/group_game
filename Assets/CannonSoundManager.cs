using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CannonSoundManager : MonoBehaviour
{
    public static CannonSoundManager Instance;
    public AudioClip[] cannonSounds; // Array to hold different cannon sounds based on damage levels
    private AudioSource audioSource;
    public AudioMixerGroup sfxMixerGroup;
    public float minTimeBetweenSounds = 0.1f;
    private float lastSoundTime;
    private Queue<int> soundQueue = new Queue<int>();
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayCannonSound(int damage)
    {
        int soundLevel = GetSoundLevelForDamage(damage);
        if (soundLevel >= 0 && soundLevel < cannonSounds.Length)
        {
            soundQueue.Enqueue(soundLevel);
            TryPlayNextSound();
        }
        else
        {
            Debug.LogWarning("Invalid damage level: " + damage);
        }
    }

    private void TryPlayNextSound()
    {
        if (Time.time - lastSoundTime >= minTimeBetweenSounds && soundQueue.Count > 0)
        {
            // Prioritize the highest level sound
            int highestPrioritySoundLevel = -1;
            int queueCount = soundQueue.Count;
            for (int i = 0; i < queueCount; i++)
            {
                int currentSoundLevel = soundQueue.Dequeue();
                if (currentSoundLevel > highestPrioritySoundLevel)
                {
                    highestPrioritySoundLevel = currentSoundLevel;
                }
                else
                {
                    soundQueue.Enqueue(currentSoundLevel);
                }
            }

            if (highestPrioritySoundLevel != -1)
            {
                audioSource.clip = cannonSounds[highestPrioritySoundLevel];
                audioSource.Play();
                lastSoundTime = Time.time;
            }
        }
    }

    private int GetSoundLevelForDamage(int damage)
    {
        if (damage <= 1)
        {
            return 0; // Level 1 sound
        }
        else if (damage <= 40)
        {
            return 1; // Level 2 sound
        }
        else if (damage > 40)
        {
            return 2; // Level 3 sound
        }
        else
        {
            return -1; // Invalid damage
        }
    }
}
