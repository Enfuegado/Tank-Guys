using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ServerMessageRouter
{
    private INetworkServer server;
    private ConnectionManager connectionManager;

    private Dictionary<MessageType, Action<string, TcpClient>> handlers;

    private int hostPlayerId = -1;

    public event Action<TcpClient, int> OnClientConnected;
    public event Action<TcpClient, int> OnClientDisconnected;
    public event Action<int> OnStartGameRequested;
    public event Action<TcpClient, MoveMessage> OnMoveReceived;

    public ServerMessageRouter(ConnectionManager connectionManager)
    {
        this.connectionManager = connectionManager;

        handlers = new Dictionary<MessageType, Action<string, TcpClient>>
        {
            { MessageType.Hello, HandleHello },
            { MessageType.StartGame, HandleStartGame },
            { MessageType.Move, HandleMove }
        };
    }

    public void Initialize(INetworkServer server)
    {
        this.server = server;
    }

    public void Handle(string json, TcpClient sender)
    {
        MessageWrapper wrapper = JsonUtility.FromJson<MessageWrapper>(json);

        if (handlers.TryGetValue(wrapper.type, out var handler))
        {
            handler(wrapper.json, sender);
        }
        else
        {
            Debug.LogWarning("Tipo no manejado: " + wrapper.type);
        }
    }

    private void HandleHello(string json, TcpClient sender)
    {
        int id = connectionManager.RegisterClient(sender);

        if (hostPlayerId == -1)
            hostPlayerId = id;

        AssignIdMessage assign = new AssignIdMessage
        {
            playerId = id
        };

        SendToClient(sender, assign);

        OnClientConnected?.Invoke(sender, id);

        BroadcastPlayerList();
    }

    private void HandleStartGame(string json, TcpClient sender)
    {
        if (!connectionManager.TryGetId(sender, out int id))
            return;

        OnStartGameRequested?.Invoke(id);
    }

    private void HandleMove(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<MoveMessage>(json);
        OnMoveReceived?.Invoke(sender, msg);
    }

    public void HandleDisconnect(TcpClient client)
    {
        if (!connectionManager.TryGetId(client, out int id)) return;

        connectionManager.RemoveClient(client);

        if (id == hostPlayerId)
            hostPlayerId = -1;

        OnClientDisconnected?.Invoke(client, id);

        BroadcastPlayerList();
    }

    private void SendToClient(TcpClient client, NetMessage msg)
    {
        MessageWrapper wrapper = new MessageWrapper
        {
            type = GetMessageType(msg),
            json = JsonUtility.ToJson(msg)
        };

        string json = JsonUtility.ToJson(wrapper);

        _ = server.Send(client, json);
    }

    public void Broadcast(NetMessage msg)
    {
        MessageWrapper wrapper = new MessageWrapper
        {
            type = GetMessageType(msg),
            json = JsonUtility.ToJson(msg)
        };

        string json = JsonUtility.ToJson(wrapper);

        _ = server.Broadcast(json);
    }

    private void BroadcastPlayerList()
    {
        List<int> ids = new List<int>();

        foreach (var kvp in connectionManager.GetType()
                     .GetField("clientIds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                     .GetValue(connectionManager) as Dictionary<TcpClient, int>)
        {
            ids.Add(kvp.Value);
        }

        PlayerListMessage msg = new PlayerListMessage
        {
            playerIds = ids.ToArray()
        };

        Broadcast(msg);
    }

    private MessageType GetMessageType(NetMessage msg)
    {
        if (msg is StartGameMessage) return MessageType.StartGame;
        if (msg is PlayerListMessage) return MessageType.PlayerList;
        if (msg is AssignIdMessage) return MessageType.AssignId;
        if (msg is HelloMessage) return MessageType.Hello;
        if (msg is MoveMessage) return MessageType.Move;

        throw new Exception("Tipo no registrado: " + msg.GetType());
    }

    public int GetHostId()
    {
        return hostPlayerId;
    }
}