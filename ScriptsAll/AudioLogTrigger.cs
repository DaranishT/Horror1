using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class AudioLogTrigger : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private RadioController radioController;
    private AudioSource _audioSource;

    private bool _isPlayed;
    private bool _playerInRange;

    // Memory optimization
    private static readonly int PlayerTagHash = "Player".GetHashCode();
    private Coroutine _playbackMonitor;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        Debug.Assert(_audioSource != null, "AudioSource component missing!", this);
        Debug.Assert(promptUI != null, "PromptUI reference missing!", this);

        promptUI.SetActive(false);
    }

    void Start()
    {
        if (radioController == null)
        {
            radioController = Object.FindFirstObjectByType<RadioController>();
        }
    }

    void OnDestroy()
    {
        // Cleanup coroutine
        if (_playbackMonitor != null)
        {
            StopCoroutine(_playbackMonitor);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !_isPlayed)
        {
            _playerInRange = true;
            radioController.EnterAudioLogZone(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _playerInRange = false;
            radioController.ExitAudioLogZone(this);
        }
    }

    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E) && !_isPlayed && radioController.IsRadioOn)
        {
            PlayLog();
        }
    }

    public void PlayLog()
    {
        _audioSource.Play();
        _isPlayed = true;
        radioController.StartLogPlayback();
        _playbackMonitor = StartCoroutine(MonitorPlayback());
    }

    public void StopLogAudio()
    {
        if (!_audioSource.isPlaying) return;

        _audioSource.Stop();
        _isPlayed = false;

        if (_playbackMonitor != null)
        {
            StopCoroutine(_playbackMonitor);
            _playbackMonitor = null;
        }
    }

    private IEnumerator MonitorPlayback()
    {
        while (_audioSource.isPlaying)
        {
            if (!radioController.IsRadioOn)
            {
                StopLogAudio();
                yield break;
            }
            yield return null;
        }
        radioController.StopLogPlayback();
    }
}