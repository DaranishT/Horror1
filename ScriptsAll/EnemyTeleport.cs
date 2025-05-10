using UnityEngine;

public class EnemyTeleport : MonoBehaviour
{
    public Transform player; // Assign the player's transform in the inspector
    public float minDistance = 10f; // Minimum distance to teleport
    public float maxDistance = 30f; // Maximum distance to teleport
    public float scareDistance = 5f; // Distance at which the enemy becomes visible/scares the player
    private Renderer enemyRenderer; // For toggling visibility
    private bool isVisible = false; // Tracks visibility state

    void Start()
    {
        enemyRenderer = GetComponentInChildren<Renderer>();
        MakeInvisible();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > maxDistance)
        {
            TeleportCloser();
        }
        else if (distanceToPlayer < scareDistance && !isVisible)
        {
            ScarePlayer();
        }
    }

    void TeleportCloser()
    {
        Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
        randomDirection += player.position;
        randomDirection.y = transform.position.y; // Keep on same plane

        transform.position = randomDirection;
        MakeInvisible();
        Debug.Log("Enemy teleported closer!");
    }

    void ScarePlayer()
    {
        isVisible = true;
        enemyRenderer.enabled = true;
        Debug.Log("Enemy scares the player!");
        // You can add a jumpscare sound or animation here
    }

    void MakeInvisible()
    {
        isVisible = false;
        enemyRenderer.enabled = false;
        Debug.Log("Enemy is now invisible.");
    }
}
