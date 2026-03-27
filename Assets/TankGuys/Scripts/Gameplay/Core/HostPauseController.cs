using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HostPauseController : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    private bool isPaused = false;

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private System.Collections.IEnumerator Initialize()
    {
        while (NetworkBootstrap.Instance == null ||
               NetworkBootstrap.Instance.ActiveClient == null ||
               NetworkBootstrap.Instance.ActiveClient.State.LocalPlayerId == -1)
        {
            yield return null;
        }

        if (!NetworkBootstrap.Instance.IsHost)
        {
            gameObject.SetActive(false);
            yield break;
        }

        UpdateText();
    }

    public void OnPauseClicked()
    {
        if (!NetworkBootstrap.Instance.IsHost)
            return;

        // ❌ ya NO invertimos aquí
        // solo enviamos lo contrario del estado actual REAL
        NetworkBootstrap.Instance.Send(new PauseMessage
        {
            isPaused = !isPaused
        });
    }

    public void ApplyPauseState(bool paused)
    {
        isPaused = paused;
        UpdateText();
    }

    private void UpdateText()
    {
        if (buttonText != null)
        {
            buttonText.text = isPaused ? "Resume" : "Pause";
        }
    }
}