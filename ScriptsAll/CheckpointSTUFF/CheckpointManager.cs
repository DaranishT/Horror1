using UnityEngine;
using UnityEngine.SceneManagement;

public static class CheckpointManager
{
    // Beacon data
    public static int DisabledBeaconCount { get; private set; }
    private const int CHECKPOINT_INTERVAL = 2;

    // Changed to internal set
    public static bool IsNewGame { get; internal set; } = true;
    private static bool _forceNewGameSpawn = false;

    // Player state
    public static Vector3 LastCheckpointPosition { get; private set; }
    public static string LastCheckpointScene { get; private set; }
    public static int BeaconsAtCheckpoint { get; private set; }

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        IsNewGame = !PlayerPrefs.HasKey("LastCheckpointScene");
        LoadSavedData();
    }

    public static void ForceNewGameSpawn()
    {
        _forceNewGameSpawn = true;
        IsNewGame = true;
    }
    public static void OnBeaconDisabled(Vector3 beaconPosition, int beaconIndex)
    {
        DisabledBeaconCount++;
        _forceNewGameSpawn = false;

        if (DisabledBeaconCount % CHECKPOINT_INTERVAL == 0 || DisabledBeaconCount == 7)
        {
            SaveCheckpoint(beaconPosition, beaconIndex);
        }
    }

    private static void SaveCheckpoint(Vector3 position, int beaconIndex)
    {
        LastCheckpointPosition = position;
        LastCheckpointScene = SceneManager.GetActiveScene().name;
        BeaconsAtCheckpoint = beaconIndex;
        IsNewGame = false;

        PlayerPrefs.SetInt("BeaconsDisabled", DisabledBeaconCount);
        PlayerPrefs.SetFloat("CheckpointX", position.x);
        PlayerPrefs.SetFloat("CheckpointY", position.y);
        PlayerPrefs.SetFloat("CheckpointZ", position.z);
        PlayerPrefs.SetString("LastCheckpointScene", LastCheckpointScene);
        PlayerPrefs.SetInt("LastBeaconIndex", beaconIndex);
        PlayerPrefs.Save();
    }

    private static void LoadSavedData()
    {
        DisabledBeaconCount = PlayerPrefs.GetInt("BeaconsDisabled", 0);
        LastCheckpointPosition = new Vector3(
            PlayerPrefs.GetFloat("CheckpointX", 0),
            PlayerPrefs.GetFloat("CheckpointY", 0),
            PlayerPrefs.GetFloat("CheckpointZ", 0));
        LastCheckpointScene = PlayerPrefs.GetString("LastCheckpointScene", "");
        BeaconsAtCheckpoint = PlayerPrefs.GetInt("LastBeaconIndex", 0);
    }

    public static bool HasCheckpoint()
    {
        return !_forceNewGameSpawn && !string.IsNullOrEmpty(LastCheckpointScene);
    }

    public static void ResetProgress()
    {
        DisabledBeaconCount = 0;
        IsNewGame = true;
        _forceNewGameSpawn = false;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}