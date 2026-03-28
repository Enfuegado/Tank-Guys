using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HostPauseController : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    private GameClient client;

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        while (NetworkBootstrap.Instance == null ||
               NetworkBootstrap.Instance.ActiveClient == null ||
               NetworkBootstrap.Instance.ActiveClient.State.LocalPlayerId == -1)
        {
            yield return null;
        }

        client = NetworkBootstrap.Instance.ActiveClient;

        if (!NetworkBootstrap.Instance.IsHost)
        {
            gameObject.SetActive(false);
            yield break;
        }

        pauseButton.onClick.RemoveAllListeners();
        pauseButton.onClick.AddListener(OnPauseClicked);

        UpdateText(false);
    }

    public void OnPauseClicked()
    {
        if (client == null) return;

        if (client.State.Phase == GamePhase.Ended)
            return;

        bool isPaused = client.State.Phase == GamePhase.Paused;

        NetworkBootstrap.Instance.Send(new PauseMessage
        {
            isPaused = !isPaused
        });
    }

    public void ApplyPauseState(bool paused)
    {
        UpdateText(paused);
    }

    private void UpdateText(bool paused)
    {
        if (buttonText != null)
        {
            buttonText.text = paused ? "Resume" : "Pause";
        }
    }
}