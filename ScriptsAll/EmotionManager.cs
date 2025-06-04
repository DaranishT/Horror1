using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EmotionManager : MonoBehaviour
{
    [Header("Emotion Settings")]
    public float emotionLevel = 0f;
    public float maxEmotion = 100f;
    public float decayRate = 5f;
    public float minPulseThreshold = 0.3f;

    [Header("Effect References")]
    public PulsatingEffect pulsatingEffect;
    public Volume postProcessingVolume;

    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    private MotionBlur motionBlur;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        InitializeEffects();
    }

    void InitializeEffects()
    {
        if (postProcessingVolume != null && postProcessingVolume.profile != null)
        {
            postProcessingVolume.profile.TryGet(out vignette);
            postProcessingVolume.profile.TryGet(out chromaticAberration);
            postProcessingVolume.profile.TryGet(out lensDistortion);
            postProcessingVolume.profile.TryGet(out motionBlur);
            postProcessingVolume.profile.TryGet(out colorAdjustments);
        }
    }

    void Update()
    {
        emotionLevel = Mathf.Max(0, emotionLevel - (decayRate * Time.deltaTime));
        float emotionFactor = Mathf.Clamp01(emotionLevel / maxEmotion);

        UpdatePostProcessing(emotionFactor);

        if (pulsatingEffect != null)
        {
            if (emotionFactor >= minPulseThreshold)
            {
                pulsatingEffect.UpdatePulsation(emotionFactor);
            }
            else
            {
                pulsatingEffect.ResetEffects();
            }
        }
    }

    void UpdatePostProcessing(float emotionFactor)
    {
        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(0.1f, 0.5f, emotionFactor);

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = Mathf.Lerp(0f, 0.5f, emotionFactor);

        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Lerp(0f, -0.4f, emotionFactor);

        if (motionBlur != null)
            motionBlur.intensity.value = Mathf.Lerp(0f, 1f, emotionFactor);

        if (colorAdjustments != null)
            colorAdjustments.colorFilter.value = Color.Lerp(
                Color.white,
                new Color(1f, 0.7f, 0.7f),
                emotionFactor
            );
    }

    public void IncreaseEmotion(float amount)
    {
        emotionLevel = Mathf.Min(emotionLevel + amount, maxEmotion);
    }

    public void ReduceEmotion(float amount)
    {
        emotionLevel = Mathf.Max(0, emotionLevel - amount);
    }

    public float NormalizedEmotion => Mathf.Clamp01(emotionLevel / maxEmotion);
}