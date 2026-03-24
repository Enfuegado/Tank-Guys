using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    private NetworkManager net;

    public Button createButton;
    public Button joinButton;

    private bool connecting = false;
    private bool alreadyLoaded = false;

    public TextMeshProUGUI statusText;

    void Start()
    {
        net = NetworkManagerBehaviour.Instance.net;

        net.OnDebug -= UpdateStatus;
        net.OnDebug += UpdateStatus;

        createButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();

        createButton.onClick.AddListener(OnCreateClicked);
        joinButton.onClick.AddListener(OnJoinClicked);
    }

    void Update()
    {
        if (alreadyLoaded) return;

        if (net.State.Players.Count > 0)
        {
            alreadyLoaded = true;
            SceneManager.LoadScene("Lobby");
        }
    }

    private void OnCreateClicked()
    {
        if (connecting) return;

        connecting = true;
        NetworkManagerBehaviour.Instance.CreateRoom();
    }

    private async void OnJoinClicked()
    {
        if (connecting) return;

        connecting = true;
        await NetworkManagerBehaviour.Instance.JoinRoom();
    }

    private void UpdateStatus(string msg)
    {
        if (statusText != null)
        {
            statusText.text = msg;
        }
    }

    private void OnDestroy()
    {
        if (net != null)
        {
            net.OnDebug -= UpdateStatus;
        }
    }
}