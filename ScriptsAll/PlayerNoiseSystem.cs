using UnityEngine;

namespace StarterAssets
{
    public class PlayerNoiseSystem : MonoBehaviour
    {
        public EmotionMeterUI emotionMeterUI; // Reference to the noise meter UI script
        public CharacterController characterController; // Reference to the CharacterController
        public FirstPersonController playerController; // Reference to the FirstPersonController script
        public EmotionManager emotionManager; // Reference to the EmotionManager script

        [Header("Noise Settings")]
        public float maxNoiseLevel = 100f; // Maximum noise level
        public float noiseDecayRate = 15f; // Faster decay rate
        public float walkingNoise = 30f; // Noise generated while walking
        public float runningNoise = 80f; // Noise generated while running
        public float jumpNoise = 50f; // Noise generated when jumping
        public float landingNoise = 60f; // Noise generated when landing
        public float idleThreshold = 0.1f; // Threshold for detecting idle state

        [Header("Emotion Settings")]
        public float walkingEmotionGain = 5f; // Emotion increase when walking
        public float runningEmotionGain = 15f; // Emotion increase when running
        public float jumpEmotionGain = 20f; // Emotion increase when jumping
        public float landingEmotionGain = 25f; // Emotion increase when landing

        [SerializeField] private float currentNoiseLevel = 0f; // Current noise level
        private bool wasGroundedLastFrame = true; // To detect landing

        // Public property for noise level access
        public float CurrentNoiseLevel => currentNoiseLevel;

        void Update()
        {
            float currentSpeed = characterController.velocity.magnitude; // Get the player's movement speed

            // Movement-based noise and emotion generation
            if (currentSpeed > playerController.SprintSpeed - idleThreshold)
            {
                // Player is running
                GenerateNoise(runningNoise * Time.deltaTime);
                IncreaseEmotion(runningEmotionGain * Time.deltaTime);
            }
            else if (currentSpeed > playerController.MoveSpeed - idleThreshold)
            {
                // Player is walking
                GenerateNoise(walkingNoise * Time.deltaTime);
                IncreaseEmotion(walkingEmotionGain * Time.deltaTime);
            }

            // Detect jump and landing
            if (!characterController.isGrounded && wasGroundedLastFrame)
            {
                // Player just jumped
                GenerateNoise(jumpNoise);
                IncreaseEmotion(jumpEmotionGain);
            }
            else if (characterController.isGrounded && !wasGroundedLastFrame)
            {
                // Player just landed
                GenerateNoise(landingNoise);
                IncreaseEmotion(landingEmotionGain);
            }

            wasGroundedLastFrame = characterController.isGrounded;

            // Decay noise level over time
            if (currentNoiseLevel > 0)
            {
                currentNoiseLevel -= noiseDecayRate * Time.deltaTime;
                currentNoiseLevel = Mathf.Max(currentNoiseLevel, 0); // Clamp to 0
            }

            if (emotionMeterUI != null)
            {
                float normalizedEmotion = emotionManager.emotionLevel / emotionManager.maxEmotion;
                emotionMeterUI.UpdateEmotionBar(normalizedEmotion);
            }
        }

        public void GenerateNoise(float noiseAmount)
        {
            currentNoiseLevel += noiseAmount;
            currentNoiseLevel = Mathf.Clamp(currentNoiseLevel, 0, maxNoiseLevel);
        }

        public void IncreaseEmotion(float amount)
        {
            emotionManager?.IncreaseEmotion(amount);
        }

        // New method for external noise level access
        public float GetCurrentNoiseNormalized()
        {
            return currentNoiseLevel / maxNoiseLevel;
        }
    }
}