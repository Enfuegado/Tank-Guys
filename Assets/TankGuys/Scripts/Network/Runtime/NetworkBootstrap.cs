using UnityEngine;
using System.Threading.Tasks;

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
        host.StartHost();

        InitializeGame();
    }

    public async Task JoinRoom()
    {
        var transport = new TcpTransport();
        client = new GameClient(transport);
        client.Start();

        InitializeGame();
        await Task.CompletedTask;
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