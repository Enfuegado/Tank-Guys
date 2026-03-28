using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private NetworkBootstrap net;
    private GameClient client;

    private bool connectionRejected = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Initialize(NetworkBootstrap network)
    {
        net = network;
        client = net.ActiveClient;

        if (client != null)
        {
            client.OnStartGame += HandleStartGame;
            client.OnDisconnected += HandleDisconnect;
        }
    }

    public void MarkConnectionRejected()
    {
        connectionRejected = true;
    }

    private void HandleStartGame()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            SceneManager.LoadScene("Game");
        });
    }

    private void HandleDisconnect()
    {
        if (connectionRejected)
            return;

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            NetworkBootstrap.Instance.ResetNetwork();
            SceneManager.LoadScene("MainMenu");
        });
    }
    public void ResetConnectionState()
    {
        connectionRejected = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;

        var playerManager = FindObjectOfType<PlayerManager>();

        if (playerManager != null && net != null)
        {
            playerManager.Initialize(net.State);
        }
    }

    public void TryStartGame()
    {
        if (net == null || net.State == null)
            return;

        int playerCount = net.State.Players.Count;

        if (playerCount < 2)
        {
            Debug.Log("No hay suficientes jugadores para iniciar");
            return;
        }

        net.Send(new StartGameMessage());
    }
}