using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CurvedCompass : MonoBehaviour
{
    [System.Serializable]
    public class CompassMarker
    {
        public string label;                     // e.g., N, NE, E, etc.
        public TextMeshProUGUI textElement;      // The actual UI Text
        public float angle;                      // World direction in degrees (0 = North)
    }

    [Header("Compass Settings")]
    public List<CompassMarker> markers;
    public Transform player;

    [Range(0, 180)] public float visibleAngle = 90f;  // How much of the compass is visible (like FOV)
    public float radius = 200f;        // Width of the arc
    public float curveHeight = 50f;    // Vertical arc depth
    public Vector2 center = Vector2.zero; // Position offset on canvas

    void Update()
    {
        float playerYaw = player.eulerAngles.y;

        foreach (var marker in markers)
        {
            float relativeAngle = Mathf.DeltaAngle(playerYaw, marker.angle);
            float t = relativeAngle / (visibleAngle / 2f); // Normalize from -1 to 1

            if (Mathf.Abs(t) > 1f)
            {
                marker.textElement.gameObject.SetActive(false);
                continue;
            }

            marker.textElement.gameObject.SetActive(true);
            Vector2 pos = GetCurvedPosition(t);
            marker.textElement.rectTransform.anchoredPosition = center + pos;
        }
    }

    Vector2 GetCurvedPosition(float t)
    {
        float angle = Mathf.Lerp(-visibleAngle / 2f, visibleAngle / 2f, (t + 1f) / 2f);
        float rad = angle * Mathf.Deg2Rad;
        float x = Mathf.Sin(rad) * radius;
        float y = -Mathf.Cos(rad) * curveHeight;
        return new Vector2(x, y);
    }
}
