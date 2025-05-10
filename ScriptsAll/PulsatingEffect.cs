using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PulsatingEffect : MonoBehaviour
{
    public Volume postProcessingVolume; // Reference to the Post-Processing Volume
    public float pulseSpeed = 2f; // Speed of the pulsation
    public float maxIntensity = 1f; // Maximum intensity of the effect
    public float minIntensity = 0f; // Minimum intensity of the effect

    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;

    private float currentIntensity = 0f;
    private bool increasing = true;

    void Start()
    {
        // Get the post-processing effects from the volume
        if (postProcessingVolume != null && postProcessingVolume.profile != null)
        {
            postProcessingVolume.profile.TryGet(out vignette);
            postProcessingVolume.profile.TryGet(out chromaticAberration);
            postProcessingVolume.profile.TryGet(out colorAdjustments);
        }
        else
        {
            Debug.LogError("Post-Processing Volume or Profile is not assigned.");
        }
    }

    void Update()
    {
        // Calculate the pulsation intensity
        if (increasing)
        {
            currentIntensity += pulseSpeed * Time.deltaTime;
            if (currentIntensity >= maxIntensity)
                increasing = false;
        }
        else
        {
            currentIntensity -= pulseSpeed * Time.deltaTime;
            if (currentIntensity <= minIntensity)
                increasing = true;
        }

        // Apply the intensity to the post-processing effects
        if (vignette != null)
            vignette.intensity.value = currentIntensity;

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = currentIntensity;

        if (colorAdjustments != null)
            colorAdjustments.colorFilter.value = Color.Lerp(Color.white, Color.red, currentIntensity);
    }
}