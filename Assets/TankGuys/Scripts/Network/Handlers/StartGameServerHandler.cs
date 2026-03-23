using System;
using System.Net.Sockets;
using UnityEngine;

public class StartGameServerHandler : IServerMessageHandler<StartGameMessage>
{
    private INetworkServer server;
    private ConnectionManager connectionManager;
    private Action onStartGame;

    public StartGameServerHandler(
        INetworkServer server,
        ConnectionManager connectionManager,
        Action onStartGame)
    {
        this.server = server;
        this.connectionManager = connectionManager;
        this.onStartGame = onStartGame;
    }

    public void Handle(StartGameMessage message, TcpClient sender)
    {
        if (sender != null && connectionManager.TryGetId(sender, out _))
        {
            Debug.LogWarning("CLIENTE INTENTO START_GAME");
            return;
        }

        Debug.Log("START_GAME válido");

        MessageWrapper wrapper = new MessageWrapper
        {
            type = MessageType.StartGame,
            json = JsonUtility.ToJson(message)
        };

        string json = JsonUtility.ToJson(wrapper);

        _ = server.Broadcast(json); 

        onStartGame?.Invoke();
    }
}