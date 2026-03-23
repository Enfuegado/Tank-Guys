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
        Debug.Log("LobbyUI Start ejecutado");

        if (NetworkManagerBehaviour.Instance == null)
        {
            Debug.LogError("NetworkManagerBehaviour no existe en la escena");
            return;
        }

        net = NetworkManagerBehaviour.Instance.net;

        if (net == null)
        {
            Debug.LogError("NetworkManager no inicializado");
            return;
        }

        if (net.State == null)
        {
            Debug.LogError("NetworkState es null");
            return;
        }

        net.State.OnPlayersUpdated += UpdatePlayersUI;
        net.OnDebug += AddLog;

        if (startGameButton == null)
        {
            Debug.LogError("startGameButton no asignado en el inspector");
            return;
        }

        startGameButton.onClick.RemoveAllListeners();
        startGameButton.onClick.AddListener(OnStartGameClicked);

        UpdatePlayersUI();
    }

    void OnDestroy()
    {
        if (net != null && net.State != null)
        {
            net.State.OnPlayersUpdated -= UpdatePlayersUI;
            net.OnDebug -= AddLog;
        }
    }

    private void OnStartGameClicked()
    {
        Debug.Log("BOTON START GAME PRESIONADO");

        if (NetworkManagerBehaviour.Instance != null)
        {
            NetworkManagerBehaviour.Instance.StartGameButton();
        }
        else
        {
            Debug.LogError("NetworkManagerBehaviour.Instance es null");
        }
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

    void AddLog(string msg)
    {
        if (statusText != null)
        {
            statusText.text = msg;
        }
    }
}