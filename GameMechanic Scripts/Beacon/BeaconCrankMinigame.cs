using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeaconCrankMinigame : MonoBehaviour
{
    [Header("Tablet Control")]
    public TabletManager tabletManager;
    public int beaconIndex; // Set to 0-6 for each beacon

    [Header("UI References")]
    public GameObject minigamePanel;
    public Image progressRing;
    public RectTransform crowbarSprite;
    public TMP_Text feedbackText;
    public TMP_Text interactPrompt;

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour playerCameraScript;

    [Header("Beacon References")]
    public GameObject lightBeam;
    public GameObject beamParticles;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip interactSound;
    public AudioClip crankSound;
    public AudioClip beaconBreakSound;

    [Header("Crank Settings")]
    public float rotationSpeed = 5f;
    public float progressPerDegree = 0.1f;
    public float requiredProgress = 100f;

    private float currentProgress;
    private bool isActive;
    private bool playerInRange;
    private Vector2 lastMousePos;
    private float totalRotation;
    private float lastCrankSoundTime;
    private const float soundInterval = 0.1f;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isActive && lightBeam.activeSelf)
        {
            StartMinigame();
            PlaySound(interactSound);
        }

        if (isActive)
        {
            HandleCranking();
            UpdateProgress();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                EndMinigame(false); // Player aborted with Tab
            }
        }
    }

    void HandleCranking()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePos = Input.mousePosition;

            if (lastMousePos != Vector2.zero)
            {
                float mouseDeltaX = currentMousePos.x - lastMousePos.x;
                float rotationAmount = mouseDeltaX * rotationSpeed;

                crowbarSprite.Rotate(0, 0, -rotationAmount);
                totalRotation += Mathf.Abs(rotationAmount);
                currentProgress += Mathf.Abs(rotationAmount) * progressPerDegree;

                feedbackText.text = "CRANKING...";
                feedbackText.color = Color.Lerp(Color.yellow, Color.green, currentProgress / requiredProgress);

                if (Time.time - lastCrankSoundTime > soundInterval && Mathf.Abs(mouseDeltaX) > 2f)
                {
                    PlaySound(crankSound);
                    lastCrankSoundTime = Time.time;
                }
            }

            lastMousePos = currentMousePos;
        }
        else
        {
            feedbackText.text = "DRAG LEFT/RIGHT TO CRANK";
            feedbackText.color = Color.yellow;
            lastMousePos = Vector2.zero;
        }
    }

    void StartMinigame()
    {
        if (tabletManager != null)
        {
            tabletManager.SwitchToBeaconTablet(beaconIndex);
        }

        isActive = true;
        currentProgress = 0f;
        totalRotation = 0f;
        lastMousePos = Input.mousePosition;
        lastCrankSoundTime = Time.time;

        minigamePanel.SetActive(true);
        progressRing.gameObject.SetActive(true);
        interactPrompt.gameObject.SetActive(false);
        feedbackText.gameObject.SetActive(true);

        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (playerCameraScript != null) playerCameraScript.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UpdateProgress()
    {
        currentProgress = Mathf.Clamp(currentProgress, 0, requiredProgress);
        progressRing.fillAmount = currentProgress / requiredProgress;

        if (currentProgress >= requiredProgress)
        {
            EndMinigame(true);
        }
    }

    void EndMinigame(bool success)
    {
        if (tabletManager != null)
        {
            if (!success)
            {
                tabletManager.ReturnToAudioLogTablet();
            }
            else
            {
                FindObjectOfType<RadioController>()?.OnTabletForcedOff();
            }
        }

        isActive = false;
        minigamePanel.SetActive(false);
        progressRing.gameObject.SetActive(false);
        feedbackText.gameObject.SetActive(true);

        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (playerCameraScript != null) playerCameraScript.enabled = true;

        interactPrompt.gameObject.SetActive(false);

        if (success)
        {
            lightBeam.SetActive(false);
            if (beamParticles != null) beamParticles.SetActive(false);
            PlaySound(beaconBreakSound);
            feedbackText.text = "BEACON DISABLED";
        }
        else
        {
            feedbackText.text = "ABORTED";
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<RadioController>()?.OnTabletForcedOn();

            if (lightBeam.activeSelf)
            {
                playerInRange = true;
                if (tabletManager != null)
                {
                    tabletManager.SwitchToBeaconTablet(beaconIndex);
                }
                interactPrompt.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (!isActive)
            {
                if (tabletManager != null)
                {
                    tabletManager.ReturnToAudioLogTablet();
                }
                FindObjectOfType<RadioController>()?.OnTabletForcedOff();
            }
            interactPrompt.gameObject.SetActive(false);
        }
    }
}