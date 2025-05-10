using UnityEngine;

public class RadioToggle : MonoBehaviour
{
    public GameObject radioModel; // 3D radio model
    public GameObject audioLogTablet; // UI tablet reference
    public AudioSource openSound;
    public AudioSource closeSound;
    public KeyCode toggleKey = KeyCode.R;

    private bool radioActive = false;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleRadio();
        }
    }

    private void ToggleRadio()
    {
        // Don't allow toggle if a beacon is active
        if (FindObjectOfType<TabletManager>()?.IsAnyBeaconActive() == true)
            return;

        bool newState = !radioModel.activeSelf; // Check actual state rather than tracked state

        radioModel.SetActive(newState);
        audioLogTablet.SetActive(newState);
        radioActive = newState;

        // Play sounds
        if (newState && openSound != null) openSound.Play();
        else if (!newState && closeSound != null) closeSound.Play();
    }
}