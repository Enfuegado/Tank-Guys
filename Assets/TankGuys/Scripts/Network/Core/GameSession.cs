using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : IGameMessageHandler
{
    private readonly NetworkManager net;
    private ServerMessageRouter serverRouter;

    private bool gameStarted = false;

    public GameSession(NetworkManager net, ServerMessageRouter serverRouter = null)
    {
        this.net = net;
        this.serverRouter = serverRouter;

        gameStarted = false;

        net.OnDisconnected += HandleDisconnect;

        if (serverRouter != null)
        {
            serverRouter.OnStartGameRequested += HandleStartGameRequest;
        }
    }

    public void TryStartGame()
    {
        if (gameStarted) return;

        if (net.State.Players.Count < 2)
        {
            Debug.Log("No hay suficientes jugadores");
            return;
        }

        net.Send(new StartGameMessage());
    }

    public void HandleStartGame(StartGameMessage msg)
    {
        Debug.Log("GAME SESSION INSTANCE: " + GetHashCode());
        Debug.Log("gameStarted antes: " + gameStarted);

        if (gameStarted) return;

        gameStarted = true;

        Debug.Log("GAME START");

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Debug.Log("LOAD SCENE EJECUTANDOSE EN MAIN THREAD");
            SceneManager.LoadScene("Game");
        });
    }

    private void HandleStartGameRequest(int playerId)
    {
        int hostId = serverRouter.GetHostId();

        if (playerId != hostId)
        {
            Debug.Log("NO AUTORIZADO");
            return;
        }

        serverRouter.Broadcast(new StartGameMessage());
    }

    private void HandleDisconnect()
    {
        Debug.Log("DESCONECTADO → RESET");

        gameStarted = false;

        if (NetworkManagerBehaviour.Instance != null)
        {
            NetworkManagerBehaviour.Instance.ResetNetwork();
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void Dispose()
    {
        net.OnDisconnected -= HandleDisconnect;

        if (serverRouter != null)
        {
            serverRouter.OnStartGameRequested -= HandleStartGameRequest;
        }
    }
}