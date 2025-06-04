using UnityEngine;

public abstract class BaseBeaconMinigame : MonoBehaviour
{


    public abstract void StartMinigame();
    public abstract void EndMinigame(bool success);
}