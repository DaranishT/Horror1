using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeaconCrankMinigame : MonoBehaviour
{
    [Header("Tablet Control")]
    public TabletManager tabletManager;
    public int beaconIndex;

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
    public LightBeam lightBeamController;

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

    void Start()
    {
        if (lightBeamController == null)
            lightBeamController = GetComponentInChildren<LightBeam>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isActive && lightBeamController != null && lightBeamController.IsActive)
        {
            StartMinigame();
            PlaySound(interactSound);
        }

        if (isActive) HandleMinigame();
    }

    void HandleMinigame()
    {
        HandleCranking();
        UpdateProgress();

        if (Input.GetKeyDown(KeyCode.Tab)) EndMinigame(false);
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

                UpdateFeedbackUI();
                PlayCrankSound(mouseDeltaX);
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

    void UpdateFeedbackUI()
    {
        feedbackText.text = "CRANKING...";
        feedbackText.color = Color.Lerp(Color.yellow, Color.green, currentProgress / requiredProgress);
    }

    void PlayCrankSound(float mouseDelta)
    {
        if (Time.time - lastCrankSoundTime > soundInterval && Mathf.Abs(mouseDelta) > 2f)
        {
            PlaySound(crankSound);
            lastCrankSoundTime = Time.time;
        }
    }

    void StartMinigame()
    {
        tabletManager?.SwitchToBeaconTablet(beaconIndex);
        isActive = true;
        currentProgress = 0f;
        minigamePanel.SetActive(true);
        interactPrompt.gameObject.SetActive(false);

        DisablePlayerControls();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void UpdateProgress()
    {
        currentProgress = Mathf.Clamp(currentProgress, 0, requiredProgress);
        progressRing.fillAmount = currentProgress / requiredProgress;

        if (currentProgress >= requiredProgress) EndMinigame(true);
    }

    void EndMinigame(bool success)
    {
        tabletManager?.ReturnToAudioLogTablet();
        isActive = false;
        minigamePanel.SetActive(false);

        EnablePlayerControls();
        Cursor.lockState = CursorLockMode.Locked;

        if (success && lightBeamController != null)
        {
            lightBeamController.Deactivate();
            PlaySound(beaconBreakSound);
            feedbackText.text = "BEACON DISABLED";
            CheckpointManager.OnBeaconDisabled(transform.position, beaconIndex);
        }
        else
        {
            feedbackText.text = "ABORTED";
        }
    }

    void DisablePlayerControls()
    {
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (playerCameraScript != null) playerCameraScript.enabled = false;
    }

    void EnablePlayerControls()
    {
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (playerCameraScript != null) playerCameraScript.enabled = true;
    }

    void PlaySound(AudioClip clip)
    {
        audioSource?.PlayOneShot(clip);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || (lightBeamController != null && !lightBeamController.IsActive)) return;

        playerInRange = true;
        FindObjectOfType<RadioController>()?.OnTabletForcedOn();
        tabletManager?.SwitchToBeaconTablet(beaconIndex);
        interactPrompt.gameObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        if (!isActive)
        {
            tabletManager?.ReturnToAudioLogTablet();
            FindObjectOfType<RadioController>()?.OnTabletForcedOff();
        }
        interactPrompt.gameObject.SetActive(false);
    }
}