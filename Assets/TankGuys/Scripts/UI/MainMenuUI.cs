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

        createButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();

        createButton.onClick.AddListener(OnCreateClicked);
        joinButton.onClick.AddListener(OnJoinClicked);
    }

    void Update()
    {
        if (alreadyLoaded) return;

        if (net.State != null && net.State.Players.Count > 0)
        {
            alreadyLoaded = true;
            SceneManager.LoadScene("Lobby");
        }
    }

    private void OnCreateClicked()
    {
        if (connecting) return;

        connecting = true;
        statusText.text = "Creando sala...";

        NetworkManagerBehaviour.Instance.CreateRoom();
    }

    private async void OnJoinClicked()
    {
        if (connecting) return;

        connecting = true;
        statusText.text = "Conectando...";

        await NetworkManagerBehaviour.Instance.JoinRoom();
    }
}