using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System.Linq;

public class LobbyUI : MonoBehaviour 
{ 
    public TextMeshProUGUI playersText; 
    public TextMeshProUGUI statusText;
    public Button startGameButton;

    void Start()
    {
        startGameButton.onClick.RemoveAllListeners();
        startGameButton.onClick.AddListener(OnStartGameClicked);
    }

    void Update()
    {
        UpdatePlayersUI();
    }

    private void OnStartGameClicked()
    {
        GameManager.Instance?.TryStartGame();
    }

    void UpdatePlayersUI()
    {
        var state = NetworkBootstrap.Instance.State;

        if (state == null) return;

        StringBuilder sb = new StringBuilder();

        foreach (var kvp in state.Players.OrderBy(p => p.Key))
        {
            sb.AppendLine("Jugador " + kvp.Key);
        }

        playersText.text = sb.ToString();
        statusText.text = "Esperando jugadores...";
    }
}