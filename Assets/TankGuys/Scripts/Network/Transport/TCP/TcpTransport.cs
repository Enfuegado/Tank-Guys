using System;
using UnityEngine;

public class TcpTransport : ITransport
{
    private TcpClientRuntime TcpClientRuntime;
    private MessageRouter router;

    public event Action<NetMessage> OnMessage;
    public event Action OnDisconnected;

    public TcpTransport()
    {
        router = new MessageRouter();

        router.Register<AssignIdMessage>(MessageType.AssignId, msg => OnMessage?.Invoke(msg));
        router.Register<PlayerListMessage>(MessageType.PlayerList, msg => OnMessage?.Invoke(msg));
        router.Register<StartGameMessage>(MessageType.StartGame, msg => OnMessage?.Invoke(msg));
        router.Register<MoveMessage>(MessageType.Move, msg => OnMessage?.Invoke(msg));
        router.Register<ShootMessage>(MessageType.Shoot, msg => OnMessage?.Invoke(msg));
        router.Register<DamageMessage>(MessageType.Damage, msg => OnMessage?.Invoke(msg));
        router.Register<PlayerStateMessage>(MessageType.PlayerState, msg => OnMessage?.Invoke(msg));
        router.Register<TurretRotationMessage>(MessageType.TurretRotation, msg => OnMessage?.Invoke(msg));
        router.Register<TankDirectionMessage>(MessageType.TankDirection, msg =>{OnMessage?.Invoke(msg);});

        TcpClientRuntime = new TcpClientRuntime(router, new NetworkClient());

        TcpClientRuntime.OnDisconnected += () =>
        {
            OnDisconnected?.Invoke();
        };
    }

    public async void Start()
    {
        await TcpClientRuntime.Connect("127.0.0.1", 7777);
    }

    public void Stop()
    {
        TcpClientRuntime.Disconnect();
    }

    public void Send(NetMessage message)
    {
        MessageWrapper wrapper = new MessageWrapper
        {
            type = GetMessageType(message),
            json = JsonUtility.ToJson(message)
        };

        string json = JsonUtility.ToJson(wrapper);
        TcpClientRuntime.Send(json);
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

        throw new Exception("Tipo no registrado: " + msg.GetType());
    }
}