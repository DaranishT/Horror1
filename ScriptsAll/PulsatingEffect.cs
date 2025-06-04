using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PulsatingEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    public float minPulseThreshold = 0.3f;
    public float basePulseSpeed = 1f;
    public float maxPulseSpeed = 3f;
    public float minIntensity = 0.1f;
    public float maxIntensity = 1f;

    [Header("References")]
    public Volume postProcessingVolume;

    // Internal components
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;
    private float currentIntensity;
    private bool increasing = true;

    void Start()
    {
        InitializeEffects();
        currentIntensity = 0f;
    }

    void InitializeEffects()
    {
        if (postProcessingVolume != null && postProcessingVolume.profile != null)
        {
            postProcessingVolume.profile.TryGet(out vignette);
            postProcessingVolume.profile.TryGet(out chromaticAberration);
            postProcessingVolume.profile.TryGet(out colorAdjustments);
        }
        ResetEffects();
    }

    // Changed to public so EmotionManager can call it
    public void UpdatePulsation(float emotionPercentage)
    {
        if (emotionPercentage < minPulseThreshold)
        {
            ResetEffects();
            return;
        }

        float scaledPulseSpeed = Mathf.Lerp(basePulseSpeed, maxPulseSpeed, emotionPercentage);
        float targetMaxIntensity = Mathf.Lerp(minIntensity, maxIntensity, emotionPercentage);

        if (increasing)
        {
            currentIntensity += scaledPulseSpeed * Time.deltaTime;
            if (currentIntensity >= targetMaxIntensity) increasing = false;
        }
        else
        {
            currentIntensity -= scaledPulseSpeed * Time.deltaTime;
            if (currentIntensity <= minIntensity) increasing = true;
        }

        ApplyEffects(currentIntensity);
    }

    // Changed to public
    public void ResetEffects()
    {
        currentIntensity = 0f;
        if (vignette != null) vignette.intensity.value = 0f;
        if (chromaticAberration != null) chromaticAberration.intensity.value = 0f;
        if (colorAdjustments != null) colorAdjustments.colorFilter.value = Color.white;
    }

    private void ApplyEffects(float intensity)
    {
        if (vignette != null) vignette.intensity.value = intensity;
        if (chromaticAberration != null) chromaticAberration.intensity.value = intensity;
        if (colorAdjustments != null)
            colorAdjustments.colorFilter.value = Color.Lerp(Color.white, Color.red, intensity);
    }
}