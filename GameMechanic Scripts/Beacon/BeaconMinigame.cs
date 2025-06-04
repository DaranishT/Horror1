using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeaconMinigame : MonoBehaviour
{
    [Header("Tablet Control")]
    public TabletManager tabletManager;
    public int beaconIndex;

    [Header("UI References")]
    public GameObject minigamePanel;
    public Image progressRing;
    public Image leftArrowImage;
    public Image rightArrowImage;
    public TMP_Text feedbackText;
    public TMP_Text interactPrompt;
    public TMP_Text directionText;

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour playerCameraScript;

    [Header("Beacon References")]
    public LightBeam lightBeamController;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip interactSound;
    public AudioClip correctInputSound;
    public AudioClip wrongInputSound;
    public AudioClip beaconBreakSound;

    [Header("Settings")]
    public float requiredProgress = 100f;
    public float progressPerHit = 8f;
    public float failDrainAmount = 15f;
    public float passiveDrainRate = 5f;
    public float timePressureWindow = 1.5f;
    public float cooldownTime = 0.15f;

    private float currentProgress;
    private bool isActive;
    private bool playerInRange;
    private float lastInputTime;
    private int currentDirection;
    private float timePressureTimer;

    void Start()
    {
        leftArrowImage?.gameObject.SetActive(false);
        rightArrowImage?.gameObject.SetActive(false);

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

        if (isActive) HandleActiveMinigame();
    }

    void HandleActiveMinigame()
    {
        HandleTimePressure();
        HandleKeyInput();
        UpdateProgress();

        if (Input.GetKeyDown(KeyCode.Tab)) EndMinigame(false);
    }

    void HandleTimePressure()
    {
        timePressureTimer -= Time.deltaTime;
        if (timePressureTimer <= 0)
        {
            currentProgress -= passiveDrainRate * Time.deltaTime;
            feedbackText.text = "FASTER!";
            feedbackText.color = Color.yellow;
        }
    }

    void HandleKeyInput()
    {
        if (Time.time - lastInputTime < cooldownTime) return;

        bool correctKeyPressed = (currentDirection == -1 && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))) ||
                               (currentDirection == 1 && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)));

        if (correctKeyPressed) HandleCorrectInput();
        else if (Input.anyKeyDown) HandleWrongInput();
    }

    void HandleCorrectInput()
    {
        currentProgress += progressPerHit;
        timePressureTimer = timePressureWindow;
        feedbackText.text = "HIT!";
        feedbackText.color = Color.green;
        lastInputTime = Time.time;
        PlaySound(correctInputSound);
        SetNewTarget();
    }

    void HandleWrongInput()
    {
        currentProgress -= failDrainAmount;
        feedbackText.text = "MISS!";
        feedbackText.color = Color.red;
        lastInputTime = Time.time;
        PlaySound(wrongInputSound);
    }

    void UpdateProgress()
    {
        currentProgress = Mathf.Clamp(currentProgress, 0, requiredProgress);
        progressRing.fillAmount = currentProgress / requiredProgress;

        if (currentProgress >= requiredProgress) EndMinigame(true);
    }

    void StartMinigame()
    {
        tabletManager?.SwitchToBeaconTablet(beaconIndex);
        isActive = true;
        currentProgress = 0;
        timePressureTimer = timePressureWindow;
        minigamePanel.SetActive(true);
        interactPrompt.gameObject.SetActive(false);

        DisablePlayerControls();
        Cursor.lockState = CursorLockMode.Locked;
        SetNewTarget();
    }

    void SetNewTarget()
    {
        currentDirection = Random.Range(0, 2) * 2 - 1;
        leftArrowImage.gameObject.SetActive(currentDirection == -1);
        rightArrowImage.gameObject.SetActive(currentDirection == 1);
        directionText.text = currentDirection == -1 ? "PRESS ←" : "PRESS →";
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
            feedbackText.text = "CANCELLED";
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