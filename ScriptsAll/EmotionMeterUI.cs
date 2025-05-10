using UnityEngine;
using StarterAssets;

public class EmotionMeterUI : MonoBehaviour
{
    public RectTransform emotionBarRect;  // Assign in Inspector
    private float originalWidth;          // Stores the bar's max width
    public EmotionManager emotionManager; // Link to your EmotionManager

    void Start()
    {
        originalWidth = emotionBarRect.sizeDelta.x;

        // Optional: Initialize with 0 emotion
        UpdateEmotionBar(0f);
    }

    void Update()
    {
        // Update the UI every frame based on current emotion
        if (emotionManager != null)
        {
            float normalizedEmotion = emotionManager.emotionLevel / emotionManager.maxEmotion;
            UpdateEmotionBar(normalizedEmotion);
        }
    }

    public void UpdateEmotionBar(float emotionLevelNormalized)
    {
        float newWidth = emotionLevelNormalized * originalWidth;
        emotionBarRect.sizeDelta = new Vector2(newWidth, emotionBarRect.sizeDelta.y);
    }
}