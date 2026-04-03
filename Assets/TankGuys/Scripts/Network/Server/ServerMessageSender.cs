using System;
using System.Net.Sockets;
using UnityEngine;

public class ServerMessageSender
{
    private INetworkServer server;

    public ServerMessageSender(INetworkServer server)
    {
        this.server = server;
    }

    public void Send(TcpClient client, NetMessage msg)
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
        if (msg is KickRequestMessage) return MessageType.Kick;
        if (msg is BanRequestMessage) return MessageType.Ban;
        if (msg is ConnectionRejectedMessage) return MessageType.ConnectionRejected;
        if (msg is KickedMessage) return MessageType.Kicked;
        if (msg is BannedMessage) return MessageType.Banned;
        if (msg is PingMessage) return MessageType.Ping;
        if (msg is PongMessage) return MessageType.Pong;
        if (msg is PingReportMessage) return MessageType.PingReport;

        throw new Exception("Tipo no registrado: " + msg.GetType());
    }
}