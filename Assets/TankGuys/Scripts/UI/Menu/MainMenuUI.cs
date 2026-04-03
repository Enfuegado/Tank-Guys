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
        statusText.text = "Creando sala...";

        NetworkBootstrap.Instance.CreateRoom();
    }

    private async void OnJoinClicked()
    {
        if (connecting) return;

        connecting = true;
        statusText.text = "Conectando...";

        await NetworkBootstrap.Instance.JoinRoom();
    }

    public void ResetUI()
    {
        connecting = false;
        statusText.text = "";
    }
}