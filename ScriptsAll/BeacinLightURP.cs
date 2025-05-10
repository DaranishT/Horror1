// BeaconLightURP.cs
using UnityEngine;
using UnityEngine.Rendering; // Required for ShadowCastingMode
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class BeaconLightURP : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color baseColor = new Color(2f, 1.8f, 1f);
    public float pulseSpeed = 1f;
    public float pulseIntensity = 0.3f;
    [Range(0, 1)] public float fogCutoff = 0.9f;

    private Material _material;
    private Renderer _renderer;
    private float _pulsePhase;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        CreateMaterial();
        ConfigureRenderer();
    }

    void CreateMaterial()
    {
        _material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
        {
            name = "BeaconInstance",
            color = baseColor,
            renderQueue = (int)RenderQueue.Transparent
        };

        _material.SetFloat("_Surface", 1); // Transparent
        _material.SetFloat("_Blend", 1);   // Additive
        _material.SetFloat("_FogCutoff", fogCutoff);
    }

    void ConfigureRenderer()
    {
        _renderer.material = _material;
        _renderer.shadowCastingMode = ShadowCastingMode.Off;
        _renderer.receiveShadows = false;
        _renderer.lightProbeUsage = LightProbeUsage.Off;
        _renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
    }

    void Update()
    {
        if (_material == null) return;

        _pulsePhase += Time.deltaTime * pulseSpeed;
        float pulse = 1f + Mathf.Sin(_pulsePhase) * pulseIntensity;

        _material.color = baseColor * pulse;
        _material.SetColor("_EmissionColor", baseColor * pulse * 2f);
    }

    void OnDisable()
    {
        if (_material != null)
        {
            if (Application.isPlaying)
                Destroy(_material);
            else
                DestroyImmediate(_material);
        }
    }
}