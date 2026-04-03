using UnityEngine;

public class PauseUIController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void ApplyPauseState(bool paused)
    {
        if (pausePanel != null)
            pausePanel.SetActive(paused);
    }
}