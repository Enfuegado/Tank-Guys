using System;
using System.Net.Sockets;
using UnityEngine;

public class StartGameServerHandler : IServerMessageHandler<StartGameMessage>
{
    private INetworkServer server;
    private ConnectionManager connectionManager;
    private Action onStartGame;
    private Func<int> getHostId;

    public StartGameServerHandler(
        INetworkServer server,
        ConnectionManager connectionManager,
        Action onStartGame,
        Func<int> getHostId)
    {
        this.server = server;
        this.connectionManager = connectionManager;
        this.onStartGame = onStartGame;
        this.getHostId = getHostId;
    }

    public void Handle(StartGameMessage message, TcpClient sender)
    {
        if (sender == null)
            return;

        if (!connectionManager.TryGetId(sender, out int playerId))
            return;

        if (playerId != getHostId())
        {
            Debug.LogWarning("CLIENTE NO AUTORIZADO INTENTO START_GAME");
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
    }
}