using UnityEngine;
using System.Threading.Tasks;

public class NetworkManagerBehaviour : MonoBehaviour
{
    public static NetworkManagerBehaviour Instance;

    public NetworkManager net;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        net = new NetworkManager();
    }

    public void CreateRoom()
    {

        net.StartHost();

        InitializeGame();
    }

    public async Task JoinRoom()
    {

        await net.Join("127.0.0.1", 7777);

        InitializeGame();
    }

    public void ResetNetwork()
    {
        net?.Shutdown();
    }

    private void InitializeGame()
    {
        if (GameManager.Instance == null)
        {
            new GameObject("GameManager").AddComponent<GameManager>();
        }

        GameManager.Instance.Initialize(net);
    }
}