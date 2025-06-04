using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;
    public AudioClip flashlightOnSound; // Sound for turning on
    public AudioClip flashlightOffSound; // Sound for turning off
    private AudioSource audioSource;
    private bool isOn = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;
            flashlight.enabled = isOn;

            if (isOn)
            {
                audioSource.PlayOneShot(flashlightOnSound); // Play the on sound
            }
            else
            {
                audioSource.PlayOneShot(flashlightOffSound); // Play the off sound
            }
        }
    }
}
