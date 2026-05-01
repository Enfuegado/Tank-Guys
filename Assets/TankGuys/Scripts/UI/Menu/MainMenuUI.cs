using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class MainMenuUI : MonoBehaviour
{
    public Button createButton;
    public Button joinButton;
    public TMP_InputField ipInput;
    public TextMeshProUGUI statusText;

    private bool connecting = false;
    private bool alreadyLoaded = false;

    private enum JoinStep { Idle, WaitingForInput }
    private JoinStep joinStep = JoinStep.Idle;

    void Start()
    {
        createButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();

        createButton.onClick.AddListener(OnCreateClicked);
        joinButton.onClick.AddListener(OnJoinClicked);

        SetJoinStep(JoinStep.Idle);

        string reason = GameManager.ConsumeDisconnectReason();
        if (!string.IsNullOrEmpty(reason))
            ErrorPanelUI.Instance?.Show(reason);
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

    private void OnJoinClicked()
    {
        if (connecting) return;

        switch (joinStep)
        {
            case JoinStep.Idle:
                SetJoinStep(JoinStep.WaitingForInput);
                break;

            case JoinStep.WaitingForInput:
                TryJoin();
                break;
        }
    }

    private void SetJoinStep(JoinStep step)
    {
        joinStep = step;

        bool showInput = step == JoinStep.WaitingForInput;
        ipInput.gameObject.SetActive(showInput);

        if (showInput)
        {
            if (string.IsNullOrWhiteSpace(ipInput.text))
                ipInput.text = GetLocalIPAddress();

            ipInput.Select();
        }
    }

    private void TryJoin()
    {
        string ip = ipInput.text.Trim();

        if (!IsValidIP(ip))
        {
            statusText.text = "";
            ErrorPanelUI.Instance?.Show("Invalid IP address");
            return;
        }

        connecting = true;
        statusText.text = "Connecting...";

        NetworkBootstrap.Instance.JoinRoom(ip);
        Invoke(nameof(ResetConnectionUI), 3f);
    }

    private void OnCreateClicked()
    {
        if (connecting) return;

        connecting = true;
        statusText.text = "Creating lobby...";

        NetworkBootstrap.Instance.CreateRoom();
        Invoke(nameof(ResetConnectionUI), 3f);
    }

    private bool IsValidIP(string ip) => IPAddress.TryParse(ip, out _);

    private string GetLocalIPAddress()
    {
        try
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            return (socket.LocalEndPoint as IPEndPoint)?.Address.ToString() ?? "127.0.0.1";
        }
        catch { return "127.0.0.1"; }
    }

    private void ResetConnectionUI()
    {
        if (!connecting) return;

        var state = NetworkBootstrap.Instance.State;
        if (state == null || state.Players.Count == 0)
        {
            connecting = false;
            statusText.text = "";
            SetJoinStep(JoinStep.Idle);
            NetworkBootstrap.Instance.ResetNetwork();
        }
    }

    public void ResetUI()
    {
        connecting = false;
        statusText.text = "";
        SetJoinStep(JoinStep.Idle);
    }
}