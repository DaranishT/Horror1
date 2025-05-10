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
    public Image leftArrowImage; // UI Image for left arrow
    public Image rightArrowImage; // UI Image for right arrow
    public TMP_Text feedbackText;
    public TMP_Text interactPrompt;
    public TMP_Text directionText;

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour playerCameraScript;

    [Header("Beacon References")]
    public GameObject lightBeam;
    public GameObject beamParticles;

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
        // Initialize by hiding both arrows
        if (leftArrowImage != null) leftArrowImage.gameObject.SetActive(false);
        if (rightArrowImage != null) rightArrowImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isActive && lightBeam != null && lightBeam.activeSelf)
        {
            StartMinigame();
            PlaySound(interactSound);
        }

        if (isActive)
        {
            HandleTimePressure();
            HandleKeyInput();
            UpdateProgress();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                EndMinigame(false);
            }
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void HandleTimePressure()
    {
        timePressureTimer -= Time.deltaTime;

        if (timePressureTimer <= 0)
        {
            currentProgress -= passiveDrainRate * Time.deltaTime;
            if (feedbackText != null)
            {
                feedbackText.text = "FASTER!";
                feedbackText.color = Color.yellow;
            }
        }
    }

    void HandleKeyInput()
    {
        if (Time.time - lastInputTime < cooldownTime) return;

        bool correctKeyPressed = false;

        if (currentDirection == -1 && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            correctKeyPressed = true;
        }
        else if (currentDirection == 1 && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            correctKeyPressed = true;
        }

        if (correctKeyPressed)
        {
            currentProgress += progressPerHit;
            timePressureTimer = timePressureWindow;

            if (feedbackText != null)
            {
                feedbackText.text = "HIT!";
                feedbackText.color = Color.green;
            }

            lastInputTime = Time.time;
            PlaySound(correctInputSound);
            SetNewTarget();
        }
        else if (Input.anyKeyDown)
        {
            currentProgress -= failDrainAmount;

            if (feedbackText != null)
            {
                feedbackText.text = "MISS!";
                feedbackText.color = Color.red;
            }

            lastInputTime = Time.time;
            PlaySound(wrongInputSound);
        }
    }

    void UpdateProgress()
    {
        currentProgress = Mathf.Clamp(currentProgress, 0, requiredProgress);

        if (progressRing != null)
        {
            progressRing.fillAmount = currentProgress / requiredProgress;
        }

        if (currentProgress >= requiredProgress)
        {
            EndMinigame(true);
        }
    }

    void StartMinigame()
    {
        if (tabletManager != null)
        {
            tabletManager.SwitchToBeaconTablet(beaconIndex);
        }

        isActive = true;
        currentProgress = 0;
        timePressureTimer = timePressureWindow;

        if (minigamePanel != null) minigamePanel.SetActive(true);
        if (interactPrompt != null) interactPrompt.gameObject.SetActive(false);

        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (playerCameraScript != null) playerCameraScript.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetNewTarget();
    }

    void SetNewTarget()
    {
        currentDirection = Random.Range(0, 2) * 2 - 1; // Randomly returns -1 (left) or 1 (right)

        // Show the appropriate arrow and hide the other
        if (leftArrowImage != null)
            leftArrowImage.gameObject.SetActive(currentDirection == -1);
        if (rightArrowImage != null)
            rightArrowImage.gameObject.SetActive(currentDirection == 1);

        if (directionText != null)
        {
            directionText.text = currentDirection == -1 ? "PRESS ←" : "PRESS →";
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

        if (minigamePanel != null) minigamePanel.SetActive(false);

        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (playerCameraScript != null) playerCameraScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (success)
        {
            if (lightBeam != null)
                lightBeam.SetActive(false);

            if (beamParticles != null)
                beamParticles.SetActive(false);

            PlaySound(beaconBreakSound);

            if (feedbackText != null)
                feedbackText.text = "BEACON DISABLED";
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "CANCELLED";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && lightBeam != null && lightBeam.activeSelf)
        {
            playerInRange = true;
            FindObjectOfType<RadioController>()?.OnTabletForcedOn();

            if (tabletManager != null)
            {
                tabletManager.SwitchToBeaconTablet(beaconIndex);
            }
            if (interactPrompt != null)
                interactPrompt.gameObject.SetActive(true);
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
            if (interactPrompt != null)
                interactPrompt.gameObject.SetActive(false);
        }
    }
}