using UnityEngine;

public class BeamPulse : MonoBehaviour
{
    public float pulseSpeed = 1f;
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    private Material beamMaterial;

    void Start()
    {
        beamMaterial = GetComponent<Renderer>().material;
    }

    void Update()
    {
        float emission = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(Time.time * pulseSpeed, 1));
        Color baseColor = beamMaterial.color;
        beamMaterial.SetColor("_EmissionColor", baseColor * emission);
    }
}