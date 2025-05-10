using UnityEngine;
using UnityEngine.UI;

namespace StarterAssets
{
    public class StaminaBarUI : MonoBehaviour
    {
        public RectTransform staminaBarRect; // The stamina bar's RectTransform
        public FirstPersonController playerController; // Reference to the player controller script

        private float originalWidth;  // The original width of the stamina bar

        void Start()
        {
            // Store the original width of the stamina bar
            originalWidth = staminaBarRect.sizeDelta.x;
        }

        public void UpdateStaminaBar(float staminaNormalized)
        {
            // Calculate the new width based on the current stamina
            float newWidth = staminaNormalized * originalWidth;

            // Update the stamina bar's width
            staminaBarRect.sizeDelta = new Vector2(newWidth, staminaBarRect.sizeDelta.y);
        }
    }
}
