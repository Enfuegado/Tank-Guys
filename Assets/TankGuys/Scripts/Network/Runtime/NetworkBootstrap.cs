using UnityEngine;

public class NetworkBootstrap : MonoBehaviour
{
    public static NetworkBootstrap Instance;

    private HostNetwork host;
    private GameClient client;

    public bool IsHost => host != null;

    public GameState State
    {
        get
        {
            if (host != null) return host.State;
            if (client != null) return client.State;
            return null;
        }
    }

    public GameClient ActiveClient
    {
        get
        {
            if (host != null) return host.Client;
            return client;
        }
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CreateRoom()
    {
        host = new HostNetwork();

        bool success = host.StartHost();

        if (!success)
        {
            host = null;

            ErrorPanelUI.Instance.Show("The room has already been created by another player");

            var menu = FindObjectOfType<MainMenuUI>();
            if (menu != null)
                menu.ResetUI();

            return;
        }

        InitializeGame();
    }

    public void JoinRoom(string ip)
    {
        var transport = new TcpTransport();
        client = new GameClient(transport);

        client.Connect(ip);

        InitializeGame();
    }

    public void Send(NetMessage msg)
    {
        if (host != null)
            host.Send(msg);
        else
            client?.Send(msg);
    }

    public void ResetNetwork()
    {
        host?.Shutdown();
        client?.Stop();

        host = null;
        client = null;
    }

    private void InitializeGame()
    {
        if (GameManager.Instance == null)
        {
            new GameObject("GameManager").AddComponent<GameManager>();
        }

        GameManager.Instance.Initialize(this);
    }
}