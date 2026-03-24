using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GameSession session;
    private NetworkManager net;

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

    public void Initialize(NetworkManager network)
    {
        net = network;

        CreateSession();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (net == null) return;

        CreateSession();
    }

    private void CreateSession()
    {
        if (session != null) return;

        session = new GameSession(net);
        net.SetHandler(session);
    }

    public void TryStartGame()
    {
        session?.TryStartGame();
    }
}