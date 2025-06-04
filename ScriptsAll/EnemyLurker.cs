using UnityEngine;
using System.Collections;


public class EnemyLurker : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float minDistance = 10f; // Minimum distance to maintain from the player
    public float maxDistance = 30f; // Maximum distance to keep the enemy far
    public float movementSpeed = 3f; // Speed at which the enemy moves closer
    public float disappearanceSpeed = 2f; // Speed at which the enemy fades away
    public float disappearDistance = 5f; // Distance at which the enemy disappears

    public LayerMask obstructionMask; // For checking if the player can see the enemy
    public AudioSource distantSound; // Optional: audio source for distant sounds (e.g., whispers)
    public AudioClip jumpScareSound; // Optional: jump scare sound when the enemy disappears

    private bool isVisible = false; // Tracks if the enemy is in the player's view
    private bool isMovingCloser = false; // Tracks if the enemy is moving closer
    private bool isDisappeared = false; // Tracks if the enemy has disappeared
    private bool isFadingOut = false; // Tracks if the enemy is fading out

    private Renderer enemyRenderer;
    private float originalAlpha;

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalAlpha = enemyRenderer.material.color.a; // Save the original alpha value
        }
    }

    void Update()
    {
        if (isDisappeared) return;

        // Check line-of-sight
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, maxDistance, obstructionMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                isVisible = true;
                isMovingCloser = false;
            }
            else
            {
                isVisible = false;
            }
        }
        else
        {
            isVisible = false;
        }

        // Handle behavior based on visibility
        if (!isVisible)
        {
            MoveCloser();
        }
        else
        {
            Freeze();
        }

        // Handle disappearance when within a certain range
        if (Vector3.Distance(player.position, transform.position) <= disappearDistance && !isFadingOut)
        {
            StartCoroutine(FadeOutAndDisappear());
        }
    }

    // Move the enemy closer to the player when not visible
    void MoveCloser()
    {
        if (Vector3.Distance(transform.position, player.position) > minDistance)
        {
            isMovingCloser = true;
            Vector3 targetPosition = Vector3.MoveTowards(transform.position, player.position, movementSpeed * Time.deltaTime);
            targetPosition.y = transform.position.y; // Keep enemy at the same height
            transform.position = targetPosition;

            // Optional: play distant sound when moving closer
            if (distantSound != null && !distantSound.isPlaying)
            {
                distantSound.Play();
            }
        }
        else
        {
            isMovingCloser = false;
        }
    }

    // Freeze the enemy when it is visible
    void Freeze()
    {
        isMovingCloser = false;

        // Optional: stop distant sound when frozen
        if (distantSound != null && distantSound.isPlaying)
        {
            distantSound.Stop();
        }
    }

    // Coroutine to handle fading out and disappearing
    IEnumerator FadeOutAndDisappear()
    {
        isFadingOut = true;

        // Fade out the enemy's material alpha value
        float elapsedTime = 0f;
        Color currentColor = enemyRenderer.material.color;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * disappearanceSpeed;
            float alpha = Mathf.Lerp(currentColor.a, 0f, elapsedTime);
            enemyRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        // After fade-out, disappear
        Disappear();
    }

    // Make the enemy disappear and trigger the jump scare sound
    void Disappear()
    {
        isDisappeared = true;
        gameObject.SetActive(false);

        // Optional: trigger a jump scare sound when the enemy disappears
        if (jumpScareSound != null && distantSound != null)
        {
            distantSound.clip = jumpScareSound;
            distantSound.Play();
        }

        // Reappear after a delay
        Invoke(nameof(TeleportToRandomPosition), Random.Range(5f, 10f));
    }

    // Teleport the enemy to a random distant position
    void TeleportToRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
        randomDirection.y = 0; // Keep horizontal movement
        Vector3 randomPosition = player.position + randomDirection;

        // Check terrain height and valid position
        if (Physics.Raycast(randomPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, Mathf.Infinity, obstructionMask))
        {
            transform.position = hit.point;
            isDisappeared = false;
            gameObject.SetActive(true);
        }
    }
}
