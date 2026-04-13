using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button createButton;
    public Button joinButton;
    public TextMeshProUGUI statusText;

    private bool connecting = false;
    private bool alreadyLoaded = false;

    void Start()
    {
        createButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();

        createButton.onClick.AddListener(OnCreateClicked);
        joinButton.onClick.AddListener(OnJoinClicked);

        string reason = GameManager.ConsumeDisconnectReason();
        if (!string.IsNullOrEmpty(reason))
        {
            if (ErrorPanelUI.Instance != null)
            {
                ErrorPanelUI.Instance.Show(reason);
            }
        }
    }

    void Update()
    {
        if (alreadyLoaded) return;

        var state = NetworkBootstrap.Instance.State;

        if (state != null && state.Players.Count > 0)
        {
            alreadyLoaded = true;
            SceneManager.LoadScene("Lobby");
        }
    }

    private void OnCreateClicked()
    {
        if (connecting) return;

        connecting = true;
        statusText.text = "Creating lobby...";

        NetworkBootstrap.Instance.CreateRoom();

        Invoke(nameof(ResetConnectionUI), 3f);
    }

    private void OnJoinClicked()
    {
        if (connecting) return;

        connecting = true;
        statusText.text = "Connecting...";

        NetworkBootstrap.Instance.JoinRoom();

        Invoke(nameof(ResetConnectionUI), 3f);
    }

    private void ResetConnectionUI()
    {
        if (!connecting) return;

        var state = NetworkBootstrap.Instance.State;

        if (state == null || state.Players.Count == 0)
        {
            connecting = false;
            statusText.text = "";
            NetworkBootstrap.Instance.ResetNetwork();
        }
    }

    public void ResetUI()
    {
        connecting = false;
        statusText.text = "";
    }
}