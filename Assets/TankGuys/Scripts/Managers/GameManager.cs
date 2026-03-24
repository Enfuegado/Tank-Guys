using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private NetworkManagerBehaviour net;
    private GameClient client;

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

    public void Initialize(NetworkManagerBehaviour network)
    {
        net = network;
        client = net.ActiveClient;

        if (client != null)
        {
            client.OnStartGame += HandleStartGame;
            client.OnDisconnected += HandleDisconnect;
        }
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
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            NetworkManagerBehaviour.Instance.ResetNetwork();
            SceneManager.LoadScene("MainMenu");
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var playerManager = FindObjectOfType<PlayerManager>();

        if (playerManager != null && net != null)
        {
            playerManager.Initialize(net.State);
        }
    }

    public void TryStartGame()
    {
        net?.Send(new StartGameMessage());
    }
}