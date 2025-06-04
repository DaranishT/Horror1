using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject enemyModel; // The enemy GameObject
    public Transform player; // Reference to the player
    public int logsRequiredToTrigger = 2; // Number of logs to trigger enemy behavior

    private int logsCollected = 0; // Tracks how many logs the player has activated
    private bool isChasing = false; // Tracks if the enemy is in chase mode

    [Header("Enemy Behavior Settings")]
    public float chaseSpeed = 3f; // Speed of the enemy while chasing
    public float attackRange = 2f; // Distance at which the enemy attacks
    public float teleportCooldown = 5f; // Cooldown between teleports

    private float teleportTimer = 0f;

    void Start()
    {
        if (enemyModel != null)
        {
            enemyModel.SetActive(false); // Initially hide the enemy
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform; // Auto-assign player if not set
        }
    }

    void Update()
    {
        if (isChasing)
        {
            HandleChase();
        }
    }

    public void IncrementLogCount()
    {
        logsCollected++;
        Debug.Log($"Logs collected: {logsCollected}");

        if (logsCollected >= logsRequiredToTrigger)
        {
            TriggerEnemy();
        }
    }

    void TriggerEnemy()
    {
        if (enemyModel != null)
        {
            enemyModel.SetActive(true); // Show the enemy
        }

        isChasing = true;
        Debug.Log("Enemy triggered and chasing the player!");
    }

    void HandleChase()
    {
        if (player == null) return;

        // Move towards the player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;

        // Check if enemy is in attack range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }

        // Handle teleportation cooldown
        teleportTimer += Time.deltaTime;
        if (teleportTimer >= teleportCooldown)
        {
            TeleportNearPlayer();
            teleportTimer = 0f;
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Enemy attacks the player!");
        // Implement attack logic, e.g., reducing player's health
    }

    void TeleportNearPlayer()
    {
        if (player == null) return;

        Vector3 randomOffset = new Vector3(
            Random.Range(-5f, 5f),
            0f,
            Random.Range(-5f, 5f)
        );

        transform.position = player.position + randomOffset;
        Debug.Log("Enemy teleported near the player!");
    }
}
