using UnityEngine;
using TMPro;
using System.Collections;

[DisallowMultipleComponent]
public class RadioController : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private GameObject radioModel;
    [SerializeField] private AudioSource staticSource;
    [SerializeField] private AudioSource beepSource;
    [SerializeField] private TextMeshProUGUI interactionPrompt;
    [SerializeField] private TabletManager tabletManager;

    private bool _isRadioOn;
    private bool _isNearAudioLog;
    private bool _isPlayingLog;
    private AudioLogTrigger _currentAudioLog;

    // Memory optimization
    private static readonly WaitForEndOfFrame FrameWait = new WaitForEndOfFrame();
    private Coroutine _monitorCoroutine;

    public bool IsRadioOn => _isRadioOn;

    void Awake()
    {
        // Validate all serialized fields
        Debug.Assert(radioModel != null, "RadioModel reference missing!", this);
        Debug.Assert(staticSource != null, "StaticSource reference missing!", this);
        Debug.Assert(beepSource != null, "BeepSource reference missing!", this);

        // Initial state setup
        radioModel.SetActive(false);
        interactionPrompt?.gameObject.SetActive(false);
    }

    void Start()
    {
        // Audio source optimization
        staticSource.loop = beepSource.loop = true;
        staticSource.playOnAwake = beepSource.playOnAwake = false;
        staticSource.Play();
        beepSource.Play();
        staticSource.Pause();
        beepSource.Pause();
    }

    void OnDestroy()
    {
        // Cleanup coroutine
        if (_monitorCoroutine != null)
        {
            StopCoroutine(_monitorCoroutine);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (tabletManager == null || !tabletManager.IsAnyBeaconActive())
            {
                ToggleRadio();
            }
        }
    }

    public void ToggleRadio()
    {
        _isRadioOn = !_isRadioOn;
        radioModel.SetActive(_isRadioOn);

        if (_isRadioOn)
        {
            if (_isNearAudioLog && !_isPlayingLog)
            {
                staticSource.Pause();
                beepSource.UnPause();
                if (interactionPrompt != null) interactionPrompt.gameObject.SetActive(true);
            }
            else if (!_isNearAudioLog)
            {
                staticSource.UnPause();
            }
        }
        else
        {
            staticSource.Pause();
            beepSource.Pause();
            if (interactionPrompt != null) interactionPrompt.gameObject.SetActive(false);

            if (_isPlayingLog)
            {
                StopLogPlayback();
            }
        }
    }

    public void EnterAudioLogZone(AudioLogTrigger logTrigger)
    {
        _isNearAudioLog = true;
        _currentAudioLog = logTrigger;

        if (_isRadioOn && !_isPlayingLog)
        {
            staticSource.Pause();
            beepSource.UnPause();
            if (interactionPrompt != null) interactionPrompt.gameObject.SetActive(true);
        }
    }

    public void ExitAudioLogZone(AudioLogTrigger logTrigger)
    {
        if (_currentAudioLog == logTrigger)
        {
            _isNearAudioLog = false;
            _currentAudioLog = null;
            beepSource.Pause();
            if (interactionPrompt != null) interactionPrompt.gameObject.SetActive(false);

            if (_isRadioOn && !_isPlayingLog)
            {
                staticSource.UnPause();
            }
        }
    }

    public void StartLogPlayback()
    {
        _isPlayingLog = true;
        beepSource.Pause();
        staticSource.Pause();
        if (interactionPrompt != null) interactionPrompt.gameObject.SetActive(false);
    }

    public void StopLogPlayback()
    {
        if (!_isPlayingLog) return;

        _isPlayingLog = false;

        if (_currentAudioLog != null)
        {
            _currentAudioLog.StopLogAudio();
        }
    }

    public void OnTabletForcedOn()
    {
        if (!_isRadioOn)
        {
            _isRadioOn = true;
            staticSource.UnPause();
        }
    }

    public void OnTabletForcedOff()
    {
        if (_isRadioOn)
        {
            _isRadioOn = false;
            staticSource.Pause();
            beepSource.Pause();
        }
    }
}