using UnityEngine;

public class GameManager : MonoBehaviour  
{  
    public static GameManager Instance;  

    private GameSession session;

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

    void Start()
    {
        var net = NetworkManagerBehaviour.Instance.net;

        session = new GameSession(net, net.ServerRouter);

        net.SetHandler(session);
    }

    public void TryStartGame()
    {
        session?.TryStartGame();
    }
}