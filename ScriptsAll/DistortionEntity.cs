using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SphereCollider))]
public class DistortionEntity : MonoBehaviour
{
    [Header("Stealth Settings")]
    [Range(0, 1)] public float minVisibility = 0.1f; // Barely visible when passive
    [Range(0, 1)] public float maxVisibility = 0.7f; // Max distortion when aggressive
    public float calmSpeed = 2f;
    public float huntSpeed = 6f;

    [Header("Distortion Effects")]
    public Material distortionMaterial; // Assign URP Decal or custom shader
    public ParticleSystem fogDistortionParticles; // Optional particle effect
    public Light subtleLight; // Optional light to highlight fog

    [Header("Aggression")]
    public float baseAggro = 0.3f;
    public float noiseSensitivity = 0.5f;
    private float _currentAggression;

    private DecalProjector _decalProjector;
    private Transform _player;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        // Set up decal projector (if using URP decals)
        if (distortionMaterial != null && distortionMaterial.shader.name.Contains("Decal"))
        {
            GameObject decalObj = new GameObject("DistortionDecal");
            decalObj.transform.SetParent(transform);
            decalObj.transform.localPosition = Vector3.zero;
            _decalProjector = decalObj.AddComponent<DecalProjector>();
            _decalProjector.material = distortionMaterial;
            _decalProjector.size = Vector3.one * 2f;
        }

        // Initialize particles
        if (fogDistortionParticles != null)
            fogDistortionParticles.Stop();
    }

    void Update()
    {
        UpdateAggression();
        UpdateMovement();
        UpdateEffects();
    }

    void UpdateAggression()
    {
        // Calculate aggression based on distance to player and noise
        float distanceFactor = 1 - Mathf.Clamp01(Vector3.Distance(transform.position, _player.position) / 10f);
        _currentAggression = Mathf.Clamp01(baseAggro + distanceFactor * noiseSensitivity);

        // Random aggression spikes
        if (_currentAggression > 0.5f && Random.value > 0.9f)
            _currentAggression = Mathf.Min(1f, _currentAggression + 0.2f);
    }

    void UpdateMovement()
    {
        float speed = Mathf.Lerp(calmSpeed, huntSpeed, _currentAggression);
        Vector3 direction = (_player.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, _player.position, speed * Time.deltaTime);
    }

    void UpdateEffects()
    {
        float visibility = Mathf.Lerp(minVisibility, maxVisibility, _currentAggression);

        // Decal distortion
        if (_decalProjector != null)
        {
            _decalProjector.fadeFactor = visibility;
            distortionMaterial.SetFloat("_DistortionStrength", visibility * 0.5f);
        }

        // Particle system (for fog interaction)
        if (fogDistortionParticles != null)
        {
            var main = fogDistortionParticles.main;
            main.startSize = visibility * 3f;

            if (visibility > 0.3f && !fogDistortionParticles.isPlaying)
                fogDistortionParticles.Play();
            else if (visibility <= 0.3f && fogDistortionParticles.isPlaying)
                fogDistortionParticles.Stop();
        }

        // Subtle light (if used)
        if (subtleLight != null)
        {
            subtleLight.intensity = visibility * 2f;
            subtleLight.color = Color.Lerp(Color.blue, Color.red, _currentAggression);
        }
    }

    // Visualize detection radius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}