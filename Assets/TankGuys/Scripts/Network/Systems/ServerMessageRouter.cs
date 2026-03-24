using System;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

public class ServerMessageRouter
{
    private INetworkServer server;
    private ConnectionManager connectionManager;

    public Action<int> OnStartGameRequested;

    private int hostPlayerId = -1;

    private readonly List<int> players = new List<int>();

    private Dictionary<MessageType, Action<TcpClient>> handlers;

    public ServerMessageRouter(ConnectionManager connectionManager)
    {
        this.connectionManager = connectionManager;

        handlers = new Dictionary<MessageType, Action<TcpClient>>
        {
            { MessageType.StartGame, HandleStartGameRequest },
            { MessageType.Hello, HandleHello }
        };
    }

    public void Initialize(INetworkServer server)
    {
        this.server = server;

        SendPlayerList();
    }

    public void Handle(string json, TcpClient sender)
    {
        MessageWrapper wrapper = JsonUtility.FromJson<MessageWrapper>(json);

        if (handlers.TryGetValue(wrapper.type, out var handler))
        {
            handler(sender);
        }
        else
        {
            Debug.LogWarning("Tipo no manejado: " + wrapper.type);
        }
    }

    private void HandleStartGameRequest(TcpClient sender)
    {
        if (!connectionManager.TryGetId(sender, out int playerId))
            return;

        OnStartGameRequested?.Invoke(playerId);
    }

    private void HandleHello(TcpClient sender)
    {
        int assignedId = connectionManager.RegisterClient(sender);

        if (!players.Contains(assignedId))
        {
            players.Add(assignedId);
        }

        if (hostPlayerId == -1)
        {
            hostPlayerId = assignedId;
        }

        AssignIdMessage assign = new AssignIdMessage
        {
            playerId = assignedId
        };

        SendToClient(sender, assign);

        SendPlayerList();
    }

    public void HandleDisconnect(TcpClient client)
    {
        if (!connectionManager.TryGetId(client, out int id)) return;

        connectionManager.RemoveClient(client);

        players.Remove(id);

        if (id == hostPlayerId)
        {
            hostPlayerId = -1;
        }

        SendPlayerList();
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

    private void SendPlayerList()
    {
        PlayerListMessage msg = new PlayerListMessage
        {
            playerIds = players.ToArray()
        };

        Broadcast(msg);
    }

    private MessageType GetMessageType(NetMessage msg)
    {
        if (msg is StartGameMessage) return MessageType.StartGame;
        if (msg is PlayerListMessage) return MessageType.PlayerList;
        if (msg is AssignIdMessage) return MessageType.AssignId;
        if (msg is HelloMessage) return MessageType.Hello;

        throw new Exception("Tipo no registrado: " + msg.GetType());
    }

    public int GetHostId()
    {
        return hostPlayerId;
    }
}