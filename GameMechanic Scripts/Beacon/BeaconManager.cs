using UnityEngine;
using System.Linq;

public class BeaconManager : MonoBehaviour
{
    public static Beacon[] AllBeacons { get; private set; }

    void Start()
    {
        RefreshBeaconList();
    }

    public static void RefreshBeaconList()
    {
        // Find all minigames (both types) and create Beacon structs
        AllBeacons = FindObjectsOfType<BeaconCrankMinigame>()
                   .Select(b => new Beacon(
                       b.beaconIndex,
                       b.gameObject,
                       b.lightBeamController))
                   .Concat(FindObjectsOfType<BeaconMinigame>()
                   .Select(b => new Beacon(
                       b.beaconIndex,
                       b.gameObject,
                       b.lightBeamController)))
                   .OrderBy(b => b.Index)
                   .ToArray();
    }

    public static void ReactivateBeacons(int lastCheckpointBeacon)
    {
        if (AllBeacons == null || AllBeacons.Length == 0)
        {
            Debug.LogWarning("No beacons found in BeaconManager. Refreshing list...");
            RefreshBeaconList();
            return;
        }

        foreach (Beacon beacon in AllBeacons)
        {
            if (beacon.Index > lastCheckpointBeacon && beacon.LightBeam != null)
            {
                beacon.LightBeam.Reactivate();
            }
        }
    }

    public struct Beacon
    {
        public int Index;
        public GameObject GameObject;
        public LightBeam LightBeam;

        public Beacon(int index, GameObject gameObject, LightBeam lightBeam)
        {
            Index = index;
            GameObject = gameObject;
            LightBeam = lightBeam;
        }
    }
}