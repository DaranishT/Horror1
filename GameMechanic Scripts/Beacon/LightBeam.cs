using UnityEngine;

public class LightBeam : MonoBehaviour
{
    [Header("Visual Components")]
    public MeshRenderer beamRenderer; // Your glowing cylinder
    public GameObject beamParticles; // Optional particle effect
    public bool startActive = true;

    private void Start()
    {
        SetActiveState(startActive);
    }

    public bool IsActive { get; private set; }

    public void Deactivate()
    {
        SetActiveState(false);
    }

    public void Reactivate()
    {
        SetActiveState(true);
    }

    private void SetActiveState(bool active)
    {
        IsActive = active;

        if (beamRenderer != null)
            beamRenderer.enabled = active;

        if (beamParticles != null)
            beamParticles.SetActive(active);
    }
}