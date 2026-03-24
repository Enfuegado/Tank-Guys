using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : IGameMessageHandler
{
    private readonly NetworkManager net;

    private bool gameStarted = false;

    public GameSession(NetworkManager net)
    {
        this.net = net;
        net.OnDisconnected += HandleDisconnect;
    }

    public void TryStartGame()
    {
        if (gameStarted) return;

        net.Send(new StartGameMessage());
    }

    public void HandleStartGame(StartGameMessage msg)
    {

        if (gameStarted) return;

        gameStarted = true;

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            SceneManager.LoadScene("Game");
        });
    }

    private void HandleDisconnect()
    {
        net.Shutdown();

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            SceneManager.LoadScene("MainMenu");
        });
    }
}