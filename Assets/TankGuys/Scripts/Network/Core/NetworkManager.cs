using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class NetworkManager
{
    private NetworkState state;

    private ClientSystem clientSystem;
    private ServerSystem serverSystem;
    private ServerMessageRouter serverRouter;

    private GameSession session;

    public NetworkState State => state;
    public ServerMessageRouter ServerRouter => serverRouter;

    public Action<string> OnDebug;
    public Action OnDisconnected;
    public Action OnConnected;

    private bool isHostStarted = false;

    public NetworkManager()
    {
        Initialize();
    }

    public void SetSession(GameSession session)
    {
        this.session = session;
    }

    private void Initialize()
    {
        state = new NetworkState();

        var clientRouter = new MessageRouter();

        clientRouter.Register<AssignIdMessage>(MessageType.AssignId, msg =>
        {
            state.SetMyPlayerId(msg.playerId);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                OnConnected?.Invoke();
            });
        });

        clientRouter.Register<PlayerListMessage>(MessageType.PlayerList, msg =>
        {
            if (msg.playerIds == null) return;
            state.SetPlayers(new List<int>(msg.playerIds));
        });

        clientRouter.Register<StartGameMessage>(MessageType.StartGame, msg =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                handler?.HandleStartGame(msg);
            });
        });

        INetworkClient client = new NetworkClient();
        INetworkServer server = new NetworkServer();

        var connectionManager = new ConnectionManager();
        serverRouter = new ServerMessageRouter(state, connectionManager);

        clientSystem = new ClientSystem(clientRouter, client);
        serverSystem = new ServerSystem(serverRouter, server);

        clientSystem.OnDebug += (msg) => OnDebug?.Invoke(msg);
        serverSystem.OnDebug += (msg) => OnDebug?.Invoke(msg);

        clientSystem.OnDisconnected += HandleDisconnect;
    }

    private void HandleDisconnect()
    {
        OnDisconnected?.Invoke();
    }

    public void ResetState()
    {
        if (clientSystem != null)
        {
            clientSystem.OnDisconnected -= HandleDisconnect;
            clientSystem.Disconnect();
        }

        if (serverSystem != null)
        {
            serverSystem.Stop(); 
        }

        clientSystem = null;
        serverSystem = null;
        serverRouter = null;
        handler = null;

        OnDebug = null;
        OnConnected = null;
        OnDisconnected = null;

        isHostStarted = false;

        Initialize();
    }

    public async Task ConnectToHost(string ip, int port)
    {
        await clientSystem.Connect(ip, port);
    }

    public void StartHost(int port)
    {
        if (isHostStarted) return;

        isHostStarted = true;

        serverSystem.Start(port);
        _ = clientSystem.Connect("127.0.0.1", port);
    }

    public void Send(NetMessage msg)
    {
        MessageWrapper wrapper = new MessageWrapper
        {
            type = GetMessageType(msg),
            json = JsonUtility.ToJson(msg)
        };

        string finalJson = JsonUtility.ToJson(wrapper);

        clientSystem?.Send(finalJson);
    }

    private MessageType GetMessageType(NetMessage msg)
    {
        if (msg is StartGameMessage) return MessageType.StartGame;
        if (msg is PlayerListMessage) return MessageType.PlayerList;
        if (msg is AssignIdMessage) return MessageType.AssignId;
        if (msg is HelloMessage) return MessageType.Hello;

        throw new Exception("Tipo de mensaje no registrado: " + msg.GetType());
    }

    private IGameMessageHandler handler;

        public void SetHandler(IGameMessageHandler handler)
        {
            this.handler = handler;
        }
}