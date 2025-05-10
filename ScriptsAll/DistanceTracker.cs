using UnityEngine;
using TMPro;

public class DistanceTracker : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // Assign player's transform in Inspector
    public TMP_Text distanceText;    // Assign your TextMeshPro UI element
    public GameObject[] audioLogs;   // Populate with audio log GameObjects

    [Header("Settings")]
    public float updateInterval = 0.3f;

    private float _timer;

    void Start()
    {
        // Safety checks
        if (player == null)
            Debug.LogError("Player transform not assigned!", this);

        if (distanceText == null)
            Debug.LogError("Distance Text not assigned!", this);
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= updateInterval)
        {
            _timer = 0;
            UpdateNearestLogDistance();
        }
    }

    void UpdateNearestLogDistance()
    {
        if (audioLogs == null || audioLogs.Length == 0)
        {
            distanceText.text = "NO LOGS FOUND";
            return;
        }

        float nearestDistance = float.MaxValue;
        foreach (GameObject log in audioLogs)
        {
            if (log == null) continue;
            float dist = Vector3.Distance(player.position, log.transform.position);
            nearestDistance = Mathf.Min(nearestDistance, dist);
        }

        distanceText.text = $"NEAREST LOG: {nearestDistance:F1}m";
    }
}
