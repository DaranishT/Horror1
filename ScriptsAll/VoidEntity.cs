using UnityEngine;
using UnityEngine.Rendering.Universal;
using StarterAssets; // Add this namespace reference

public class VoidEntity : MonoBehaviour
{
    [Header("Entity Settings")]
    public float minSize = 0.5f;
    public float maxSize = 3f;
    public float calmSpeed = 2f;
    public float huntSpeed = 8f;
    public float detectionRadius = 15f;
    public float waypointRadius = 10f;

    [Header("Distortion Effect")]
    public Material distortionMaterial;
    public float maxDistortion = 0.5f;

    [Header("Emotion Response")]
    public float baseAggro = 0.3f;
    public float emotionMultiplier = 2f;
    public float noiseSensitivity = 0.5f;

    [Header("References")]
    public FirstPersonController playerController;
    public PlayerNoiseSystem noiseSystem;
    public EmotionManager emotionManager;

    private DecalProjector _distortionProjector;
    private Vector3 _targetPosition;
    private float _currentAggression;

    void Start()
    {
        // Set up visual effect
        GameObject distortionObj = new GameObject("DistortionEffect");
        distortionObj.transform.SetParent(transform);
        distortionObj.transform.localPosition = Vector3.zero;

        _distortionProjector = distortionObj.AddComponent<DecalProjector>();
        _distortionProjector.material = distortionMaterial;
        _distortionProjector.size = Vector3.one * minSize;

        // Initial random target
        _targetPosition = GetRandomWaypoint();

        // Auto-find references if not set
        if (playerController == null)
            playerController = FindObjectOfType<FirstPersonController>();
        if (noiseSystem == null)
            noiseSystem = FindObjectOfType<PlayerNoiseSystem>();
        if (emotionManager == null)
            emotionManager = FindObjectOfType<EmotionManager>();
    }

    void Update()
    {
        if (playerController == null || noiseSystem == null || emotionManager == null)
        {
            Debug.LogWarning("Missing references in VoidEntity!");
            return;
        }

        UpdateAggression();
        UpdateMovement();
        UpdateVisuals();
    }

    void UpdateAggression()
    {
        // Get normalized emotion level (0-1)
        float emotionFactor = (emotionManager.emotionLevel / emotionManager.maxEmotion) * emotionMultiplier;

        // Get normalized noise level (0-1)
        float noiseFactor = (noiseSystem.GetCurrentNoiseNormalized()) * noiseSensitivity;

        _currentAggression = Mathf.Clamp01(baseAggro + emotionFactor + noiseFactor);

        // Random intensity spikes when highly agitated
        if (_currentAggression > 0.7f && Random.value > 0.95f)
        {
            _currentAggression = Mathf.Min(1f, _currentAggression + 0.3f);
        }
    }

    void UpdateMovement()
    {
        float currentSpeed = Mathf.Lerp(calmSpeed, huntSpeed, _currentAggression);

        if (_currentAggression > 0.5f)
        {
            // Hunt mode: move toward player with prediction
            Vector3 playerDirection = (playerController.transform.position - transform.position).normalized;
            _targetPosition = playerController.transform.position + playerDirection * 2f;
        }
        else if (Vector3.Distance(transform.position, _targetPosition) < 1f || Random.value < 0.01f)
        {
            // Patrol mode: random waypoints
            _targetPosition = GetRandomWaypoint();
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPosition,
            currentSpeed * Time.deltaTime
        );
    }

    void UpdateVisuals()
    {
        // Size scales with aggression
        float currentSize = Mathf.Lerp(minSize, maxSize, _currentAggression);
        _distortionProjector.size = Vector3.one * currentSize;

        // Distortion intensity
        float distortion = Mathf.Lerp(0.1f, maxDistortion, _currentAggression);
        distortionMaterial.SetFloat("_Distortion", distortion);

        // Pulsing effect
        float pulse = Mathf.Sin(Time.time * 3f) * 0.1f + 1f;
        _distortionProjector.fadeFactor = _currentAggression * pulse;
    }

    Vector3 GetRandomWaypoint()
    {
        return transform.position + Random.insideUnitSphere * waypointRadius;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Increase player emotion when entity is close
            emotionManager.IncreaseEmotion(0.2f * Time.deltaTime);
        }
    }
}