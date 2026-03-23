using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    }

    void Start()
    {
        if (NetworkManagerBehaviour.Instance != null)
        {
            net = NetworkManagerBehaviour.Instance.net;

            net.OnStartGameReceived -= HandleStartGame;
            net.OnStartGameReceived += HandleStartGame;
        }
        else
        {
            Debug.LogError("NetworkManagerBehaviour no encontrado");
        }
    }

    public void TryStartGame()
    {
        if (net == null) return;

        if (net.State.Players.Count < 2)
        {
            Debug.Log("NO HAY SUFICIENTES JUGADORES");
            return;
        }

        StartGameMessage msg = new StartGameMessage();
        net.Send(msg);
    }

    private void HandleStartGame()
    {
        Debug.Log("GAME START RECIBIDO → CAMBIANDO ESCENA");
        SceneManager.LoadScene("Game");
    }
}