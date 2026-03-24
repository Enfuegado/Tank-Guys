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
        Debug.Log("CREATE ROOM CLICK");
        net.StartHost(7777);
    }

    public async Task JoinRoom()
    {
        Debug.Log("JOIN ROOM CLICK");
        await net.ConnectToHost("127.0.0.1", 7777);
    }

    public void ResetNetwork()
    {
        net?.ResetState();
    }
}