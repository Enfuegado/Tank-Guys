using System;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

public class ServerMessageRouter
{
    private NetworkState state;
    private INetworkServer server;
    private ConnectionManager connectionManager;

    public Action<int> OnStartGameRequested;

    private int hostPlayerId = -1;

    public ServerMessageRouter(NetworkState state, ConnectionManager connectionManager)
    {
        this.state = state;
        this.connectionManager = connectionManager;
    }

    public void Initialize(INetworkServer server)
    {
        this.server = server;

        SendPlayerList();
    }

    public void Handle(string json, TcpClient sender)
    {
        MessageWrapper wrapper = JsonUtility.FromJson<MessageWrapper>(json);

        switch (wrapper.type)
        {
            case MessageType.StartGame:
            {
                HandleStartGameRequest(sender);
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

    private void HandleStartGameRequest(TcpClient sender)
    {
        if (!connectionManager.TryGetId(sender, out int playerId))
            return;

        OnStartGameRequested?.Invoke(playerId);
    }

    private void HandleHello(TcpClient sender)
    {
        int assignedId = connectionManager.RegisterClient(sender);

        if (hostPlayerId == -1)
        {
            hostPlayerId = assignedId;
        }

        state.AddPlayer(assignedId);

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
        state.RemovePlayer(id);

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
            playerIds = new List<int>(state.Players).ToArray()
        };

        Debug.Log("ENVIANDO PLAYER_LIST");

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