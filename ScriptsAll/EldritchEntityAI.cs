using UnityEngine;
using UnityEngine.AI;

public class EldritchEntityAI : MonoBehaviour
{
    [Header("Navigation")]
    public NavMeshAgent agent;
    public float roamRadius = 20f;
    private float _nextRoamTime;
    private Vector3 _lastKnownPlayerPosition;

    [Header("Chasing")]
    public Transform player;
    public float chaseSpeed = 6f;
    public float chaseDuration = 5f; // Time to keep chasing after losing sight
    private float _chaseEndTime;
    private bool _isChasing;

    [Header("Detection")]
    public float detectionRadius = 15f;
    public LayerMask obstructionLayers;
    public float visionAngle = 90f;

    [Header("Emotion Response")]
    public EmotionManager emotionManager;
    public float emotionChaseThreshold = 50f;

    [Header("BeamLight Response")]
    public float speedBoostPerBeam = 1f;
    private int _disabledBeams;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _lastKnownPlayerPosition = transform.position;
        SetRandomDestination();
    }

    void Update()
    {
        if (ShouldChasePlayer())
        {
            StartChasing();
        }

        if (_isChasing)
        {
            // Always update destination while chasing
            agent.SetDestination(player.position);
            agent.speed = chaseSpeed + (_disabledBeams * speedBoostPerBeam);

            // Stop chasing if player is hidden for too long
            if (Time.time > _chaseEndTime)
            {
                StopChasing();
            }
        }
        else if (agent.remainingDistance < 1f || Time.time > _nextRoamTime)
        {
            SetRandomDestination();
        }
    }

    bool ShouldChasePlayer()
    {
        // Chase if beacons are disabled
        if (_disabledBeams > 0) return true;

        // Chase if emotion is high
        if (emotionManager.emotionLevel > emotionChaseThreshold) return true;

        // Chase if player is visible
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if (angle < visionAngle / 2f)
            {
                if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRadius, obstructionLayers))
                {
                    if (hit.transform == player)
                    {
                        _lastKnownPlayerPosition = player.position;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += _lastKnownPlayerPosition;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            _nextRoamTime = Time.time + Random.Range(3f, 8f);
        }
    }

    void StartChasing()
    {
        _isChasing = true;
        agent.speed = chaseSpeed + (_disabledBeams * speedBoostPerBeam);
        _chaseEndTime = Time.time + chaseDuration;
    }

    void StopChasing()
    {
        _isChasing = false;
        _lastKnownPlayerPosition = player.position; // Remember where we last saw player
        SetRandomDestination();
    }

    public void OnBeamLightDisabled()
    {
        _disabledBeams++;
        StartChasing();
    }
}