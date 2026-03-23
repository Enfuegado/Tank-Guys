using System;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

public class ServerMessageRouter
{
    private NetworkState state;
    private INetworkServer server;
    private ConnectionManager connectionManager;

    private Dictionary<Type, Action<NetMessage, TcpClient>> handlers = new();

    public Action OnStartGame;

    public ServerMessageRouter(NetworkState state, ConnectionManager connectionManager)
    {
        this.state = state;
        this.connectionManager = connectionManager;
    }

    public void Initialize(INetworkServer server)
    {
        this.server = server;

        Register(new StartGameServerHandler(
            server,
            connectionManager,
            () => OnStartGame?.Invoke()
        ));

        int hostId = 1;
        state.myPlayerId = hostId;
        state.AddPlayer(hostId);

        SendPlayerList();
    }

    public void Register<T>(IServerMessageHandler<T> handler) where T : NetMessage
    {
        handlers[typeof(T)] = (msg, sender) =>
        {
            handler.Handle((T)msg, sender);
        };
    }

    public void Handle(string json, TcpClient sender)
    {
        MessageWrapper wrapper = JsonUtility.FromJson<MessageWrapper>(json);

        switch (wrapper.type)
        {
            case MessageType.PlayerList:
            {
                var msg = JsonUtility.FromJson<PlayerListMessage>(wrapper.json);
                if (handlers.TryGetValue(typeof(PlayerListMessage), out var handler))
                    handler(msg, sender);
                break;
            }

            case MessageType.StartGame:
            {
                var msg = JsonUtility.FromJson<StartGameMessage>(wrapper.json);
                if (handlers.TryGetValue(typeof(StartGameMessage), out var handler))
                    handler(msg, sender);
                break;
            }

            case MessageType.Hello:
            {
                HandleHello(sender);
                break;
            }

            default:
                Debug.LogWarning("Tipo no manejado: " + wrapper.type);
                break;
        }
    }

    private void HandleHello(TcpClient sender)
    {
        int assignedId = connectionManager.RegisterClient(sender);

        state.AddPlayer(assignedId);

        AssignIdMessage assign = new AssignIdMessage
        {
            playerId = assignedId
        };

        MessageWrapper wrapper = new MessageWrapper
        {
            type = MessageType.AssignId,
            json = JsonUtility.ToJson(assign)
        };

        string json = JsonUtility.ToJson(wrapper);

        _ = server.Send(sender, json);

        SendPlayerList();
    }

    public void HandleDisconnect(TcpClient client)
    {
        if (!connectionManager.TryGetId(client, out int id)) return;

        connectionManager.RemoveClient(client);
        state.RemovePlayer(id);

        SendPlayerList();
    }

    private void SendPlayerList()
    {
        PlayerListMessage msg = new PlayerListMessage
        {
            playerIds = new List<int>(state.Players).ToArray()
        };

        MessageWrapper wrapper = new MessageWrapper
        {
            type = MessageType.PlayerList,
            json = JsonUtility.ToJson(msg)
        };

        string json = JsonUtility.ToJson(wrapper);

        Debug.Log("ENVIANDO PLAYER_LIST");

        _ = server.Broadcast(json);
    }
}