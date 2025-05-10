using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    public AudioClip[] footstepSounds; // Array of footstep sounds
    public AudioClip jumpSound; // Sound for jumping
    public AudioClip landingSound; // Sound for landing
    private AudioSource audioSource;
    private CharacterController characterController;
    private bool wasGrounded;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        wasGrounded = characterController.isGrounded;
    }

    void Update()
    {
        PlayFootstepSounds();
        PlayJumpLandingSounds();
    }

    void PlayFootstepSounds()
    {
        if (characterController.isGrounded && characterController.velocity.magnitude > 0.1f && !audioSource.isPlaying)
        {
            // Play a random footstep sound
            int index = Random.Range(0, footstepSounds.Length);
            audioSource.clip = footstepSounds[index];
            audioSource.Play();
        }
    }

    void PlayJumpLandingSounds()
    {
        if (!wasGrounded && characterController.isGrounded)
        {
            // Play landing sound
            audioSource.PlayOneShot(landingSound);
        }
        else if (wasGrounded && !characterController.isGrounded)
        {
            // Play jump sound
            audioSource.PlayOneShot(jumpSound);
        }

        wasGrounded = characterController.isGrounded;
    }
}
