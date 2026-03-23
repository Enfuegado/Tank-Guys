using UnityEngine;

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
        net.StartHost(7777);
    }

    public async void JoinRoom()
    {
        await net.ConnectToHost("127.0.0.1", 7777);
    }

    public void StartGameButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TryStartGame();
        }
        else
        {
            Debug.LogError("GameManager no existe en la escena");
        }
    }
}