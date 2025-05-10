using UnityEngine;

public class TabletCoordinator : MonoBehaviour
{
    [System.Serializable]
    public class TabletDisplay
    {
        public GameObject tabletObject;
        public Camera tabletCamera;
        public RenderTexture renderTexture;
        [HideInInspector] public int beaconID = -1;
    }

    public TabletDisplay permanentTablet; // Audio log tablet
    public TabletDisplay[] beaconTablets = new TabletDisplay[7]; // For 7 beacons

    private TabletDisplay _currentActiveTablet;

    void Start()
    {
        InitializeTablets();
    }

    void InitializeTablets()
    {
        // Set up permanent tablet
        permanentTablet.tabletCamera.targetTexture = permanentTablet.renderTexture;
        permanentTablet.tabletObject.SetActive(true);
        permanentTablet.tabletCamera.enabled = true;
        _currentActiveTablet = permanentTablet;

        // Set up beacon tablets
        for (int i = 0; i < beaconTablets.Length; i++)
        {
            beaconTablets[i].tabletCamera.targetTexture = beaconTablets[i].renderTexture;
            beaconTablets[i].tabletObject.SetActive(false);
            beaconTablets[i].tabletCamera.enabled = false;
            beaconTablets[i].beaconID = i; // Assign IDs
        }
    }

    public void ActivateBeaconTablet(int beaconID)
    {
        if (beaconID < 0 || beaconID >= beaconTablets.Length) return;

        // Deactivate current tablet
        _currentActiveTablet.tabletObject.SetActive(false);
        _currentActiveTablet.tabletCamera.enabled = false;

        // Activate new tablet
        beaconTablets[beaconID].tabletObject.SetActive(true);
        beaconTablets[beaconID].tabletCamera.enabled = true;
        _currentActiveTablet = beaconTablets[beaconID];
    }

    public void ReturnToPermanentTablet()
    {
        if (_currentActiveTablet == permanentTablet) return;

        _currentActiveTablet.tabletObject.SetActive(false);
        _currentActiveTablet.tabletCamera.enabled = false;

        permanentTablet.tabletObject.SetActive(true);
        permanentTablet.tabletCamera.enabled = true;
        _currentActiveTablet = permanentTablet;
    }

    public void ForceResetTablets()
    {
        // Called when player tabs out mid-game
        if (_currentActiveTablet != permanentTablet)
        {
            _currentActiveTablet.tabletObject.SetActive(false);
            _currentActiveTablet.tabletCamera.enabled = false;
        }

        permanentTablet.tabletObject.SetActive(true);
        permanentTablet.tabletCamera.enabled = true;
        _currentActiveTablet = permanentTablet;
    }
}