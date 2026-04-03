using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ServerMessageProcessor
{
    private INetworkServer server;
    private ConnectionManager connectionManager;
    private GameState gameState;

    private ServerAdminService adminService;
    private ServerConnectionService connectionService;
    private ServerMessageSender messageSender;

    private Dictionary<MessageType, Action<string, TcpClient>> handlers;

    private int hostPlayerId = -1;

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

        adminService = new ServerAdminService(connectionManager);
        adminService.OnClientDisconnected += (c, id) => OnClientDisconnected?.Invoke(c, id);
        adminService.OnPlayerListChanged += BroadcastPlayerList;

        connectionService = new ServerConnectionService(connectionManager, gameState);
        connectionService.OnClientConnected += (c, id) => OnClientConnected?.Invoke(c, id);

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
            { MessageType.PingReport, HandlePingReport }
        };
    }

    public void Initialize(INetworkServer server)
    {
        this.server = server;
        messageSender = new ServerMessageSender(server);
    }

    public void Handle(string json, TcpClient sender)
    {
        MessageWrapper wrapper = JsonUtility.FromJson<MessageWrapper>(json);

        if (handlers.TryGetValue(wrapper.type, out var handler))
        {
            handler(wrapper.json, sender);
        }
    }

    private void HandleHello(string json, TcpClient sender)
    {
        connectionService.TryHandleConnection(
            sender,
            adminService.IsBanned,
            messageSender.Send,
            BroadcastPlayerList,
            ref hostPlayerId
        );
    }

    private void HandleKick(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<KickRequestMessage>(json);

        if (!IsHost(sender)) return;
        if (msg.targetId == hostPlayerId) return;

        var client = connectionManager.GetClientById(msg.targetId);
        if (client == null) return;

        messageSender.Send(client, new KickedMessage
        {
            reason = "You have been kicked by the host"
        });

        adminService.KickClient(msg.targetId);
    }

    private void HandleBan(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<BanRequestMessage>(json);

        if (!IsHost(sender)) return;
        if (msg.targetId == hostPlayerId) return;

        var client = connectionManager.GetClientById(msg.targetId);
        if (client == null) return;

        messageSender.Send(client, new BannedMessage
        {
            reason = "You have been banned by the host"
        });

        adminService.BanClient(msg.targetId);
    }

    private bool IsHost(TcpClient sender)
    {
        return connectionManager.TryGetId(sender, out int id) && id == hostPlayerId;
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

    public void HandleDisconnect(TcpClient client)
    {
        if (!connectionManager.TryGetId(client, out int id)) return;

        connectionManager.RemoveClient(client);

        if (id == hostPlayerId)
            hostPlayerId = -1;

        OnClientDisconnected?.Invoke(client, id);

        BroadcastPlayerList();
    }

    private void HandlePing(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<PingMessage>(json);

        if (!connectionManager.TryGetId(sender, out int id))
            return;

        messageSender.Send(sender, new PongMessage
        {
            playerId = id,
            timestamp = msg.timestamp
        });
    }

    private void HandlePingReport(string json, TcpClient sender)
    {
        var msg = JsonUtility.FromJson<PingReportMessage>(json);
        messageSender.Broadcast(msg);
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

        messageSender.Broadcast(msg);
    }

    public int GetHostId()
    {
        return hostPlayerId;
    }
    public void Broadcast(NetMessage msg)
    {
        messageSender.Broadcast(msg);
    }
}