using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Buttons")]
    public GameObject continueButton;

    void Start()
    {
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        continueButton.SetActive(CheckpointManager.HasCheckpoint());
    }

    public void OnContinue()
    {
        SceneManager.LoadScene(CheckpointManager.LastCheckpointScene);
    }

    public void OnPlay()
    {
        CheckpointManager.ResetProgress();
        CheckpointManager.ForceNewGameSpawn();
        SceneManager.LoadScene("GameScene");
    }

    public void OnOptions() => TogglePanel(optionsPanel);
    public void OnCredits() => TogglePanel(creditsPanel);
    public void CloseOptionsPanel() => optionsPanel.SetActive(false);
    public void CloseCreditsPanel() => creditsPanel.SetActive(false);

    void TogglePanel(GameObject panel)
    {
        optionsPanel.SetActive(panel == optionsPanel);
        creditsPanel.SetActive(panel == creditsPanel);
    }

    public void OnQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}