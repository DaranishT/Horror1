using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BeaconSkillCheck : MonoBehaviour
{
    [Header("UI References")]
    public GameObject skillCheckPanel;
    public Image progressRing;
    public RectTransform needle;
    public RectTransform successZone;
    public GameObject interactPrompt;
    public Image successZoneImage;

    [Header("Settings")]
    public float needleSpeed = 180f;
    public float zoneSize = 45f;
    public float zoneSpawnDelay = 2f;
    public float stabilizationTime = 5f;
    public Color successColor = Color.green;
    public Color failColor = Color.red;

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript;

    private bool isActive = false;
    private float currentProgress = 0f;
    private float targetAngle;
    private bool isZoneActive = false;
    private bool playerInRange = false;
    private Color originalZoneColor;

    void Start()
    {
        originalZoneColor = successZoneImage.color;
        ForceUIState(false);
        ResetNeedlePosition();
    }

    void Update()
    {
        HandleInteractions();
        HandleEscape();
    }

    void ResetNeedlePosition()
    {
        needle.localRotation = Quaternion.Euler(0, 0, -90f);
    }

    void HandleInteractions()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isActive)
        {
            StartStabilization();
        }

        if (isActive)
        {
            HandleMinigame();
            UpdateProgress();
        }
    }

    void HandleEscape()
    {
        if (isActive && Input.GetKeyDown(KeyCode.Escape))
        {
            StabilizationComplete();
            Debug.Log("Minigame canceled by player");
        }
    }

    void ForceUIState(bool state)
    {
        skillCheckPanel.SetActive(state);
        successZone.gameObject.SetActive(state);
        interactPrompt.SetActive(!state);
    }

    void HandleMinigame()
    {
        needle.Rotate(0, 0, needleSpeed * Time.deltaTime);
    }

    bool IsNeedleInZone()
    {
        float needleAngle = NormalizeAngle(needle.eulerAngles.z);
        float zoneStart = NormalizeAngle(targetAngle - (zoneSize / 2));
        float zoneEnd = NormalizeAngle(targetAngle + (zoneSize / 2));

        DebugVisuals(needleAngle, zoneStart, zoneEnd);

        return (needleAngle >= zoneStart && needleAngle <= zoneEnd) ||
               (zoneStart > zoneEnd && (needleAngle >= zoneStart || needleAngle <= zoneEnd));
    }

    float NormalizeAngle(float angle)
    {
        return (angle % 360 + 360) % 360;
    }

    void DebugVisuals(float needleAngle, float zoneStart, float zoneEnd)
    {
        Debug.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, needleAngle) * Vector3.up * 2, Color.white);
        Debug.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, zoneStart) * Vector3.up * 2, Color.cyan);
        Debug.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, zoneEnd) * Vector3.up * 2, Color.magenta);
    }

    IEnumerator SpawnNewZone()
    {
        isZoneActive = false;
        successZone.gameObject.SetActive(false);
        yield return new WaitForSeconds(zoneSpawnDelay);

        targetAngle = Random.Range(0, 360);
        successZone.rotation = Quaternion.Euler(0, 0, targetAngle);
        successZone.gameObject.SetActive(true);
        isZoneActive = true;
    }

    void UpdateProgress()
    {
        progressRing.fillAmount = Mathf.Clamp01(currentProgress / stabilizationTime);
        if (progressRing.fillAmount >= 1f) StabilizationComplete();
    }

    void Success()
    {
        currentProgress += stabilizationTime / 4f;
        StartCoroutine(FlashZone(successColor));
        StartCoroutine(SpawnNewZone());
    }

    void Fail()
    {
        currentProgress = Mathf.Max(0, currentProgress - (stabilizationTime / 5f));
        StartCoroutine(FlashZone(failColor));
    }

    IEnumerator FlashZone(Color flashColor)
    {
        successZoneImage.color = flashColor;
        yield return new WaitForSeconds(0.2f);
        successZoneImage.color = originalZoneColor;
    }

    void StartStabilization()
    {
        isActive = true;
        currentProgress = 0f;
        ResetNeedlePosition();
        ForceUIState(true);

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(SpawnNewZone());
        Debug.Log("Minigame started");
    }

    void StabilizationComplete()
    {
        isActive = false;
        ForceUIState(false);
        ResetNeedlePosition();

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Minigame ended");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            playerInRange = true;
            interactPrompt.SetActive(true);
            Debug.Log("Player entered beacon zone");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactPrompt.SetActive(false);
            Debug.Log("Player left beacon zone");
        }
    }
}