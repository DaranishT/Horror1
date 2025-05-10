using UnityEngine;

public class SafeZoneEmotionReset : MonoBehaviour
{
    [Header("Emotion Reset")]
    public EmotionManager emotionManager; // Drag & drop the player's EmotionManager here

    [Header("Visual Effects")]
    public Light safeZoneLight; // Assign a child light GameObject
    public ParticleSystem floatingOrbs; // Assign a child ParticleSystem

    [Header("Audio")]
    public AudioClip calmSound; // Assign in Inspector
    private AudioSource audioSource;

    [Header("Settings")]
    public float lightIntensity = 3f;
    public Color lightColor = Color.cyan;

    private void Start()
    {
        // Set up light
        if (safeZoneLight != null)
        {
            safeZoneLight.intensity = lightIntensity;
            safeZoneLight.color = lightColor;
        }

        // Set up audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // 3D sound
        audioSource.playOnAwake = false;

        // Start particles
        if (floatingOrbs != null)
            floatingOrbs.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && emotionManager != null)
        {
            emotionManager.emotionLevel = 0; // Instantly reset emotion
            PlayCalmSound();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && emotionManager != null)
            emotionManager.emotionLevel = 0; // Lock emotion to 0
    }

    private void PlayCalmSound()
    {
        if (calmSound != null && audioSource != null)
            audioSource.PlayOneShot(calmSound);
    }
}