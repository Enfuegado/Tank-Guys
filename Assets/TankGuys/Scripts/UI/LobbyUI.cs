using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class LobbyUI : MonoBehaviour 
{ 
    public TextMeshProUGUI playersText; 
    public TextMeshProUGUI statusText;
    public Button startGameButton;

    private NetworkManager net;

    void Start()
    {
        net = NetworkManagerBehaviour.Instance.net;

        if (net == null || net.State == null)
        {
            Debug.LogError("Network no inicializado");
            return;
        }

        net.State.OnPlayersUpdated += UpdatePlayersUI;

        startGameButton.onClick.RemoveAllListeners();
        startGameButton.onClick.AddListener(OnStartGameClicked);

        UpdatePlayersUI();
    }

    void OnDestroy()
    {
        if (net != null && net.State != null)
        {
            net.State.OnPlayersUpdated -= UpdatePlayersUI;
        }
    }

    private void OnStartGameClicked()
    {
        GameManager.Instance?.TryStartGame();
    }

    void UpdatePlayersUI()
    {
        if (net == null || net.State == null) return;

        StringBuilder sb = new StringBuilder();

        foreach (int id in net.State.Players)
        {
            sb.AppendLine("Jugador " + id);
        }

        playersText.text = sb.ToString();
        statusText.text = "Esperando jugadores...";
    }
}