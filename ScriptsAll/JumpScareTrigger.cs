using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class JumpScareTrigger : MonoBehaviour
{
    public VideoPlayer jumpscareVideo;
    public RawImage jumpscareImage;
    public float fadeSpeed = 2f;
    public float delayBeforeQuit = 5f;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(PlayJumpScare());
        }
    }

    IEnumerator PlayJumpScare()
    {
        jumpscareVideo.Prepare();

        while (!jumpscareVideo.isPrepared)
        {
            yield return null;
        }

        // Fade in the RawImage
        Color color = jumpscareImage.color;
        float alpha = 0;

        while (alpha < 1)
        {
            alpha += Time.unscaledDeltaTime * fadeSpeed;
            color.a = alpha;
            jumpscareImage.color = color;
            yield return null;
        }

        // Freeze game time
        Time.timeScale = 0f;

        // Play video with unscaled time
        jumpscareVideo.Play();

        // Wait in real time (since game is frozen)
        yield return new WaitForSecondsRealtime((float)jumpscareVideo.length + delayBeforeQuit);

        // Quit game
        Application.Quit();

#if UNITY_EDITOR
        // For testing inside the editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
