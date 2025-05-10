using UnityEngine;

public class ProximityReveal : MonoBehaviour
{
    public Transform player;
    public float revealDistance = 10f;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            meshRenderer.enabled = false; // Start invisible
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (meshRenderer != null)
            meshRenderer.enabled = distance < revealDistance;
    }
}
