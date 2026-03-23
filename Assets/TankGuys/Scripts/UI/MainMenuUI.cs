using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    private NetworkManager net;

    private bool connecting = false;
    private bool loadingLobby = false;

    public TextMeshProUGUI statusText;

    void Start()
    {
        net = NetworkManagerBehaviour.Instance.net;

        net.OnDebug += UpdateStatus;
    }

    void Update()
    {
        if (!loadingLobby && net.State.myPlayerId != -1)
        {
            loadingLobby = true;
            SceneManager.LoadScene("Lobby");
        }
    }

    public void CreateRoom()
    {
        if (connecting) return;

        connecting = true;
        net.StartHost(7777);
    }

    public async void JoinRoom()
    {
        if (connecting) return;

        connecting = true;
        await net.ConnectToHost("127.0.0.1", 7777);
    }

    private void UpdateStatus(string msg)
    {
        if (statusText != null)
        {
            statusText.text = msg;
        }
    }
}