using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HostPauseController : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    private bool isPaused = false;

    private float lastPauseTime = 0f;
    private float cooldown = 0.2f;

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
        var client = NetworkBootstrap.Instance.ActiveClient;

        if (!NetworkBootstrap.Instance.IsHost)
            return;

        if (client.State.Phase == GamePhase.Ended)
            return;

        if (Time.time - lastPauseTime < cooldown)
            return;

        lastPauseTime = Time.time;

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