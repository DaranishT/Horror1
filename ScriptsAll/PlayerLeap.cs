using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(FirstPersonController))]
public class PlayerLeap : MonoBehaviour
{
    [Header("Leap Settings")]
    public float minLeapForce = 10f;
    public float maxLeapForce = 30f;
    public float maxChargeTime = 2f;
    public float leapDuration = 0.5f;
    public float upwardForceMultiplier = 0.3f;
    public KeyCode leapKey = KeyCode.LeftControl;
    public float staminaCostMultiplier = 0.5f;
    public float leapCooldown = 1f;
    public float chargeSlowdownFactor = 0.5f;

    [Header("UI Settings")]
    public GameObject chargeBarContainer; // Parent object that holds the charge UI
    public Image chargeFillImage;        // The actual fillable image component

    // Private variables
    private float chargeTimer = 0f;
    private bool isCharging = false;
    private CharacterController characterController;
    private FirstPersonController fpsController;
    private Camera playerCamera;
    private float leapSpeed;
    private bool isLeaping = false;
    private float leapTimer = 0f;
    private float cooldownTimer = 0f;
    private bool canLeap = true;
    private bool wasGroundedLastFrame = true;
    private Vector3 currentLeapDirection;
    private float originalMoveSpeed;
    private float originalSprintSpeed;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        fpsController = GetComponent<FirstPersonController>();
        playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            Debug.LogWarning("Player camera not found in children, using Camera.main");
        }

        // Store original movement speeds
        originalMoveSpeed = fpsController.MoveSpeed;
        originalSprintSpeed = fpsController.SprintSpeed;

        // Initialize UI
        ResetChargeUI();
    }

    void Update()
    {
        HandleCooldown();
        HandleGroundedCheck();
        HandleCharging();
        HandleLeaping();

        // Update charge UI
        if (isCharging && chargeFillImage != null)
        {
            chargeFillImage.fillAmount = chargeTimer / maxChargeTime;
        }
    }

    private void HandleCooldown()
    {
        if (!canLeap)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canLeap = true;
            }
        }
    }

    private void HandleGroundedCheck()
    {
        bool isGrounded = fpsController.Grounded;

        if (!wasGroundedLastFrame && isGrounded)
        {
            canLeap = true;
        }

        wasGroundedLastFrame = isGrounded;
    }

    private void HandleCharging()
    {
        // Start charging when key is pressed and conditions are met
        if (Input.GetKeyDown(leapKey) && fpsController.Grounded && canLeap && !isLeaping && fpsController.Stamina > 0)
        {
            StartCharging();
        }

        // Continue charging while key is held
        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            chargeTimer = Mathf.Clamp(chargeTimer, 0f, maxChargeTime);

            // Continuously update leap direction based on current camera view
            UpdateLeapDirection();

            // Release leap when key is released or stamina runs out
            if (Input.GetKeyUp(leapKey) || fpsController.Stamina <= 0)
            {
                TryPerformLeap();
            }
        }
    }

    private void UpdateLeapDirection()
    {
        // Get current camera forward direction
        currentLeapDirection = playerCamera.transform.forward;
        // Apply upward force while keeping mostly horizontal movement
        currentLeapDirection.y = Mathf.Max(currentLeapDirection.y, upwardForceMultiplier);
        currentLeapDirection.Normalize();
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeTimer = 0f;
        UpdateLeapDirection();

        // Show charge UI
        if (chargeBarContainer != null)
        {
            chargeBarContainer.SetActive(true);
        }
        if (chargeFillImage != null)
        {
            chargeFillImage.fillAmount = 0f;
        }

        // Slow down movement while charging
        fpsController.MoveSpeed = originalMoveSpeed * chargeSlowdownFactor;
        fpsController.SprintSpeed = originalSprintSpeed * chargeSlowdownFactor;
    }

    private void TryPerformLeap()
    {
        if (!isCharging) return;

        float chargePercent = chargeTimer / maxChargeTime;
        float staminaCost = chargePercent * fpsController.MaxStamina * staminaCostMultiplier;

        if (fpsController.Stamina >= staminaCost)
        {
            PerformLeap(chargePercent, staminaCost);
        }
        else
        {
            CancelLeap();
        }
    }

    private void PerformLeap(float chargePercent, float staminaCost)
    {
        fpsController.DepleteStamina(staminaCost);

        // Calculate leap force
        leapSpeed = Mathf.Lerp(minLeapForce, maxLeapForce, chargePercent);

        // Start leap
        isLeaping = true;
        leapTimer = 0f;

        // Reset movement speeds
        fpsController.MoveSpeed = originalMoveSpeed;
        fpsController.SprintSpeed = originalSprintSpeed;

        // Start cooldown
        canLeap = false;
        cooldownTimer = leapCooldown;

        // Hide charge UI
        ResetChargeUI();
    }

    private void HandleLeaping()
    {
        if (!isLeaping) return;

        // Update direction continuously during leap
        UpdateLeapDirection();

        leapTimer += Time.deltaTime;

        if (leapTimer < leapDuration)
        {
            // Apply movement with gradual slowdown
            float speedMultiplier = 1f - (leapTimer / leapDuration);
            Vector3 move = currentLeapDirection * leapSpeed * speedMultiplier * Time.deltaTime;

            // Apply gravity from FirstPersonController
            move.y += fpsController.Gravity * Time.deltaTime;

            characterController.Move(move);
        }
        else
        {
            EndLeap();
        }
    }

    private void CancelLeap()
    {
        isCharging = false;
        fpsController.MoveSpeed = originalMoveSpeed;
        fpsController.SprintSpeed = originalSprintSpeed;
        ResetChargeUI();
    }

    private void EndLeap()
    {
        isLeaping = false;
        isCharging = false;
    }

    private void ResetChargeUI()
    {
        if (chargeBarContainer != null)
        {
            chargeBarContainer.SetActive(false);
        }
        if (chargeFillImage != null)
        {
            chargeFillImage.fillAmount = 0f;
        }
    }

    void OnDisable()
    {
        if (isLeaping || isCharging)
        {
            EndLeap();
            CancelLeap();
        }
    }
}