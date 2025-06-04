using UnityEngine;
using TMPro; // Required for TextMeshPro
using StarterAssets;

public class FlameEmotionTrigger : MonoBehaviour
{
    [Header("Emotion Settings")]
    public float emotionIncreaseAmount = 30f;
    public float tickInterval = 0.5f;

    [Header("UI Settings")]
    public TMP_Text flameWarningText; // Assign in Inspector

    private float nextTickTime;
    private PlayerNoiseSystem playerNoiseSystem;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNoiseSystem = other.GetComponent<PlayerNoiseSystem>();
            if (flameWarningText != null)
                flameWarningText.gameObject.SetActive(true); // Show warning
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerNoiseSystem != null)
        {
            if (Time.time >= nextTickTime)
            {
                playerNoiseSystem.IncreaseEmotion(emotionIncreaseAmount * tickInterval);
                nextTickTime = Time.time + tickInterval;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNoiseSystem = null;
            if (flameWarningText != null)
                flameWarningText.gameObject.SetActive(false); // Hide warning
        }
    }
}