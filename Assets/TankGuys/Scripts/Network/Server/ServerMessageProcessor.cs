using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ServerMessageProcessor
{
    private INetworkServer server;
    private ConnectionManager connectionManager;
    private GameState gameState;

    private Dictionary<MessageType, Action<string, TcpClient>> handlers;

    private int hostPlayerId = -1;

    private HashSet<string> bannedIPs = new HashSet<string>();

    public event Action<TcpClient, int> OnClientConnected;
    public event Action<TcpClient, int> OnClientDisconnected;
    public event Action<int> OnStartGameRequested;
    public event Action<TcpClient, MoveMessage> OnMoveReceived;
    public event Action<TcpClient, ShootMessage> OnShootReceived;
    public event Action<TcpClient, DamageMessage> OnDamageReceived;
    public event Action<TcpClient, TurretRotationMessage> OnTurretRotationReceived;
    public event Action<TcpClient, TankDirectionMessage> OnTankDirectionReceived;
    public event Action<TcpClient, PauseMessage> OnPauseReceived;

    public ServerMessageProcessor(ConnectionManager connectionManager, GameState state)
    {
        this.connectionManager = connectionManager;
        this.gameState = state;

        handlers = new Dictionary<MessageType, Action<string, TcpClient>>
        {
            { MessageType.Hello, HandleHello },
            { MessageType.StartGame, HandleStartGame },
            { MessageType.Move, HandleMove },
            { MessageType.Shoot, HandleShoot },
            { MessageType.Damage, HandleDamage },
            { MessageType.TurretRotation, HandleTurretRotation },
            { MessageType.TankDirection, HandleTankDirection },
            { MessageType.Pause, HandlePause },
            { MessageType.Kick, HandleKick },
            { MessageType.Ban, HandleBan },
            { MessageType.Ping, HandlePing },
            { MessageType.Pong, HandlePong },
            { MessageType.PingReport, HandlePingReport }
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
            Debug.LogError("Tipo no registrado: " + wrapper.type);
        }
    }

    private void HandlePing(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<PingMessage>(json);

        if (!connectionManager.TryGetId(sender, out int id))
            return;

        PongMessage pong = new PongMessage
        {
            playerId = id,
            timestamp = msg.timestamp
        };

        SendToClient(sender, pong);
    }

    private void HandlePong(string json, TcpClient sender)
    {
    }
    private void HandlePingReport(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<PingReportMessage>(json);

        Broadcast(msg);
    }

    private void HandleHello(string json, TcpClient sender)
    {
        string ip = ((System.Net.IPEndPoint)sender.Client.RemoteEndPoint).Address.ToString();

        if (gameState.Phase != GamePhase.Lobby)
        {
            SendToClient(sender, new ConnectionRejectedMessage
            {
                reason = "La partida ya ha comenzado"
            });

            System.Threading.Tasks.Task.Delay(100).ContinueWith(_ =>
            {
                try { sender.Close(); } catch {}
            });

            return;
        }

        if (bannedIPs.Contains(ip))
        {
            SendToClient(sender, new ConnectionRejectedMessage
            {
                reason = "Has sido baneado del host"
            });

            System.Threading.Tasks.Task.Delay(100).ContinueWith(_ =>
            {
                try { sender.Close(); } catch {}
            });

            return;
        }

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

    private void HandleShoot(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<ShootMessage>(json);
        OnShootReceived?.Invoke(sender, msg);
    }

    private void HandleDamage(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<DamageMessage>(json);
        OnDamageReceived?.Invoke(sender, msg);
    }

    private void HandleTurretRotation(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<TurretRotationMessage>(json);
        OnTurretRotationReceived?.Invoke(sender, msg);
    }

    private void HandleTankDirection(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<TankDirectionMessage>(json);
        OnTankDirectionReceived?.Invoke(sender, msg);
    }

    private void HandlePause(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<PauseMessage>(json);
        OnPauseReceived?.Invoke(sender, msg);
    }

    private void HandleKick(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<KickPlayerMessage>(json);

        if (!connectionManager.TryGetId(sender, out int senderId))
            return;

        if (senderId != hostPlayerId)
            return;

        if (msg.targetId == hostPlayerId)
            return;

        KickClient(msg.targetId);
    }

    private void HandleBan(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<BanPlayerMessage>(json);

        if (!connectionManager.TryGetId(sender, out int senderId))
            return;

        if (senderId != hostPlayerId)
            return;

        if (msg.targetId == hostPlayerId)
            return;

        BanClient(msg.targetId);
    }

    private void KickClient(int targetId)
    {
        var client = connectionManager.GetClientById(targetId);
        if (client == null) return;

        client.Close();
        connectionManager.RemoveClient(client);

        OnClientDisconnected?.Invoke(client, targetId);

        BroadcastPlayerList();
    }

    private void BanClient(int targetId)
    {
        var client = connectionManager.GetClientById(targetId);
        if (client == null) return;

        string ip = ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

        bannedIPs.Add(ip);

        KickClient(targetId);
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

        foreach (var kvp in connectionManager.GetAll())
        {
            ids.Add(kvp.Value);
        }

        ids.Sort();

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
        if (msg is ShootMessage) return MessageType.Shoot;
        if (msg is DamageMessage) return MessageType.Damage;
        if (msg is PlayerStateMessage) return MessageType.PlayerState;
        if (msg is TurretRotationMessage) return MessageType.TurretRotation;
        if (msg is TankDirectionMessage) return MessageType.TankDirection;
        if (msg is PauseMessage) return MessageType.Pause;
        if (msg is GameEndMessage) return MessageType.GameEnd;
        if (msg is KickPlayerMessage) return MessageType.Kick;
        if (msg is BanPlayerMessage) return MessageType.Ban;
        if (msg is ConnectionRejectedMessage) return MessageType.ConnectionRejected;
        if (msg is PingMessage) return MessageType.Ping;
        if (msg is PongMessage) return MessageType.Pong;
        if (msg is PingReportMessage) return MessageType.PingReport;

        throw new Exception("Tipo no registrado: " + msg.GetType());
    }

    public int GetHostId()
    {
        return hostPlayerId;
    }
}