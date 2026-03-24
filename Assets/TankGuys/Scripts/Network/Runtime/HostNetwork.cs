using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class HostNetwork : INetworkRole
{
    private NetworkState state;

    private ClientSystem clientSystem;
    private ServerSystem serverSystem;
    private ServerMessageRouter serverRouter;
    private ConnectionManager connectionManager;

    private IGameMessageHandler handler;

    public NetworkState State => state;

    public event Action OnDisconnected;

    public HostNetwork()
    {
        state = new NetworkState();
        connectionManager = new ConnectionManager();

        var clientRouter = BuildClientRouter();

        serverRouter = new ServerMessageRouter(state, connectionManager);

        clientSystem = new ClientSystem(clientRouter, new NetworkClient());
        serverSystem = new ServerSystem(serverRouter, new NetworkServer());

        clientSystem.OnDisconnected += () =>
        {
            OnDisconnected?.Invoke();
        };

        WireServerLogic();
    }

    private MessageRouter BuildClientRouter()
    {
        var router = new MessageRouter();

        router.Register<AssignIdMessage>(MessageType.AssignId, msg =>
        {
            state.SetMyPlayerId(msg.playerId);
        });

        router.Register<PlayerListMessage>(MessageType.PlayerList, msg =>
        {
            if (msg.playerIds != null)
                state.SetPlayers(new List<int>(msg.playerIds));
        });

        router.Register<StartGameMessage>(MessageType.StartGame, msg =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                handler?.HandleStartGame(msg);
            });
        });

        return router;
    }

    private void WireServerLogic()
    {
        serverRouter.OnStartGameRequested += (playerId) =>
        {
            int hostId = serverRouter.GetHostId();

            if (playerId != hostId)
                return;

            serverRouter.Broadcast(new StartGameMessage());
        };
    }

    public async Task Start()
    {
        serverSystem.Start(7777);
        await clientSystem.Connect("127.0.0.1", 7777);
    }

    public async Task Connect(string ip, int port)
    {
        throw new NotImplementedException();
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
        serverSystem.Stop();
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