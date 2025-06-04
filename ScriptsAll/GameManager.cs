using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int audioLogsCollected = 0;

    public void CollectAudioLog()
    {
        audioLogsCollected++;
    }
}
