using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using StarterAssets;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Player References")]
    public FirstPersonController playerController;
    public Transform playerCamera;

    [Header("Respawn Settings")]
    public GameObject deathScreenUI;
    public float respawnDelay = 2f;
    public Transform newGameStartPosition;

    void Start()
    {
        if (CheckpointManager.HasCheckpoint() && !CheckpointManager.IsNewGame)
        {
            RespawnAtCheckpoint(false);
        }
        else
        {
            playerController.transform.position = newGameStartPosition.position;
            playerCamera.rotation = newGameStartPosition.rotation;
        }
    }

    public void OnCaughtByEntity()
    {
        if (CheckpointManager.HasCheckpoint())
        {
            StartCoroutine(DeathSequence());
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    IEnumerator DeathSequence()
    {
        playerController.enabled = false;
        deathScreenUI.SetActive(true);
        yield return new WaitForSecondsRealtime(respawnDelay);
        RespawnAtCheckpoint(true);
    }

    void RespawnAtCheckpoint(bool resetEntity)
    {
        playerController.transform.position = CheckpointManager.LastCheckpointPosition;
        playerCamera.rotation = Quaternion.identity;
        playerController.enabled = true;
        deathScreenUI.SetActive(false);

        if (resetEntity)
        {
            EldritchEntityAI entity = FindObjectOfType<EldritchEntityAI>();
            if (entity != null) entity.StopChasing();
        }

        BeaconManager.ReactivateBeacons(CheckpointManager.BeaconsAtCheckpoint);
    }
}