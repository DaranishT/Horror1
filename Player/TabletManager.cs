using UnityEngine;

public class TabletManager : MonoBehaviour
{
    [Header("Tablet References")]
    public GameObject audioLogTablet;
    public GameObject[] beaconTablets;

    public bool IsAnyBeaconActive()
    {
        foreach (var tablet in beaconTablets)
        {
            if (tablet != null && tablet.activeSelf)
                return true;
        }
        return false;
    }

    public void SwitchToBeaconTablet(int beaconIndex)
    {
        if (beaconIndex < 0 || beaconIndex >= beaconTablets.Length) return;

        audioLogTablet?.SetActive(false);

        for (int i = 0; i < beaconTablets.Length; i++)
        {
            if (beaconTablets[i] != null)
                beaconTablets[i].SetActive(i == beaconIndex);
        }
    }

    public void ReturnToAudioLogTablet()
    {
        foreach (var tablet in beaconTablets)
            tablet?.SetActive(false);

        audioLogTablet?.SetActive(true);

        // Notify RadioController that we've forced the tablet on
        FindObjectOfType<RadioController>()?.OnTabletForcedOn();
    }
}