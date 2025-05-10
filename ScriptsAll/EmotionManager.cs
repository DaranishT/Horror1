using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EmotionManager : MonoBehaviour
{
    public float emotionLevel = 0f; // Starts at 0, increases with fear
    public float maxEmotion = 100f; // Max fear level
    public float decayRate = 5f; // How fast fear reduces

    public PulsatingEffect pulsatingEffect; // Reference to the PulsatingEffect script

    private Volume postProcessingVolume;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    private MotionBlur motionBlur;

    void Start()
    {
        postProcessingVolume = GetComponent<Volume>();

        // Try getting post-processing effects
        postProcessingVolume.profile.TryGet(out vignette);
        postProcessingVolume.profile.TryGet(out chromaticAberration);
        postProcessingVolume.profile.TryGet(out lensDistortion);
        postProcessingVolume.profile.TryGet(out motionBlur);
    }

    void Update()
    {
        // Gradually reduce emotion over time
        emotionLevel -= decayRate * Time.deltaTime;
        emotionLevel = Mathf.Clamp(emotionLevel, 0, maxEmotion);

        // Calculate intensity based on emotion
        float emotionFactor = emotionLevel / maxEmotion;

        // Update post-processing effects
        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(0.1f, 0.5f, emotionFactor); // Red tint

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = Mathf.Lerp(0f, 0.5f, emotionFactor); // Distortion

        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Lerp(0f, -0.4f, emotionFactor); // Screen warp

        if (motionBlur != null)
            motionBlur.intensity.value = Mathf.Lerp(0f, 1f, emotionFactor); // Blur effect

        // Update pulsating effect based on emotion
        if (pulsatingEffect != null)
        {
            pulsatingEffect.pulseSpeed = Mathf.Lerp(1f, 5f, emotionFactor); // Faster pulsation as emotion increases
            pulsatingEffect.maxIntensity = Mathf.Lerp(0.5f, 1f, emotionFactor); // Stronger effect as emotion increases
        }
    }

    // Call this to increase emotion (from enemy or events)
    public void IncreaseEmotion(float amount)
    {
        emotionLevel += amount;
        emotionLevel = Mathf.Clamp(emotionLevel, 0, maxEmotion);
    }

    // Add this right after your IncreaseEmotion method
    public void ReduceEmotion(float amount)
    {
        emotionLevel -= amount;
        emotionLevel = Mathf.Clamp(emotionLevel, 0, maxEmotion);

        // Optional: Add visual/audio feedback for calming effect
        Debug.Log("Calmed down! Emotion reduced by " + amount);
    }


}