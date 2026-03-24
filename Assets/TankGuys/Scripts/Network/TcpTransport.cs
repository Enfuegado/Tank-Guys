using System;
using UnityEngine;

public class TcpTransport : ITransport
{
    private ClientSystem clientSystem;
    private MessageRouter router;

    public event Action<NetMessage> OnMessage;
    public event Action OnDisconnected;

    public TcpTransport()
    {
        router = new MessageRouter();

        router.Register<AssignIdMessage>(MessageType.AssignId, msg =>
        {
            OnMessage?.Invoke(msg);
        });

        router.Register<PlayerListMessage>(MessageType.PlayerList, msg =>
        {
            OnMessage?.Invoke(msg);
        });

        router.Register<StartGameMessage>(MessageType.StartGame, msg =>
        {
            OnMessage?.Invoke(msg);
        });

        router.Register<MoveMessage>(MessageType.Move, msg =>
        {
            OnMessage?.Invoke(msg);
        });

        clientSystem = new ClientSystem(router, new NetworkClient());

        clientSystem.OnDisconnected += () =>
        {
            OnDisconnected?.Invoke();
        };
    }

    public async void Start()
    {
        await clientSystem.Connect("127.0.0.1", 7777);
    }

    public void Stop()
    {
        clientSystem.Disconnect();
    }

    public void Send(NetMessage message)
    {
        MessageWrapper wrapper = new MessageWrapper
        {
            type = GetMessageType(message),
            json = JsonUtility.ToJson(message)
        };

        string json = JsonUtility.ToJson(wrapper);

        clientSystem.Send(json);
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
}