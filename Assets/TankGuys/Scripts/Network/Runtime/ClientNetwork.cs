using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ClientNetwork : INetworkRole
{
    private NetworkState state;
    private ClientSystem clientSystem;
    private MessageRouter router;

    private IGameMessageHandler handler;

    public NetworkState State => state;

    public event Action OnDisconnected;

    public ClientNetwork()
    {
        state = new NetworkState();

        router = new MessageRouter();

        router.Register<AssignIdMessage>(MessageType.AssignId, msg =>
        {
            state.SetMyPlayerId(msg.playerId);
        });

        router.Register<PlayerListMessage>(MessageType.PlayerList, msg =>
        {
            if (msg.playerIds != null)
                state.ApplyPlayerList(new List<int>(msg.playerIds));
        });

        router.Register<StartGameMessage>(MessageType.StartGame, msg =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                handler?.HandleStartGame(msg);
            });
        });

        clientSystem = new ClientSystem(router, new NetworkClient());

        clientSystem.OnDisconnected += () =>
        {
            OnDisconnected?.Invoke();
        };
    }

    public async Task Start()
    {
    }

    public async Task Connect(string ip, int port)
    {
        await clientSystem.Connect(ip, port);
    }

    public void Send(NetMessage msg)
    {
        MessageWrapper wrapper = new MessageWrapper
        {
            type = GetMessageType(msg),
            json = JsonUtility.ToJson(msg)
        };

        string json = JsonUtility.ToJson(wrapper);

        clientSystem.Send(json);
    }

    public void Shutdown()
    {
        clientSystem.Disconnect();
    }

    public void SetHandler(IGameMessageHandler handler)
    {
        this.handler = handler;
    }

    private MessageType GetMessageType(NetMessage msg)
    {
        if (msg is StartGameMessage) return MessageType.StartGame;
        if (msg is PlayerListMessage) return MessageType.PlayerList;
        if (msg is AssignIdMessage) return MessageType.AssignId;
        if (msg is HelloMessage) return MessageType.Hello;

        throw new Exception("Tipo no registrado: " + msg.GetType());
    }
}