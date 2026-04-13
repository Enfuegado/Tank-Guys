using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerRowUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public GameObject actionsPanel;
    public Button mainButton;
    public Button kickButton;
    public Button banButton;

    private int playerId;

    public void Setup(int id)
    {
        playerId = id;
        nameText.text = "Player " + id;

        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(ToggleActions);

        kickButton.onClick.RemoveAllListeners();
        kickButton.onClick.AddListener(Kick);

        banButton.onClick.RemoveAllListeners();
        banButton.onClick.AddListener(Ban);

        actionsPanel.SetActive(false);

        var net = NetworkBootstrap.Instance;

        if (net == null || net.State == null)
        {
            mainButton.gameObject.SetActive(false);
            return;
        }

        int localId = net.State.LocalPlayerId;

        if (!net.IsHost || playerId == localId)
        {
            mainButton.gameObject.SetActive(false);
        }
        else
        {
            mainButton.gameObject.SetActive(true);
        }
    }

    void ToggleActions()
    {
        LobbyUI.Instance.CloseAll();
        actionsPanel.SetActive(true);
    }

    void Kick()
    {
        NetworkBootstrap.Instance.Send(new KickRequestMessage { targetId = playerId });
    }

    void Ban()
    {
        NetworkBootstrap.Instance.Send(new BanRequestMessage { targetId = playerId });
    }

    public void Close()
    {
        actionsPanel.SetActive(false);
    }
}