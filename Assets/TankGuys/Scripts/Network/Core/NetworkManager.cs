using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class NetworkManager
{
    private NetworkState state;

    private ClientSystem clientSystem;
    private ServerSystem serverSystem;

    private bool isHost;

    public NetworkState State => state;

    public Action<string> OnDebug;
    public Action OnStartGameReceived;

    public NetworkManager()
    {
        state = new NetworkState();

        var clientRouter = new MessageRouter();

        clientRouter.Register<AssignIdMessage>(MessageType.AssignId, msg =>
        {
            state.myPlayerId = msg.playerId;
        });

        clientRouter.Register<PlayerListMessage>(MessageType.PlayerList, msg =>
        {
            if (msg.playerIds == null)
            {
                Debug.LogError("PLAYER IDS NULL (CLIENT)");
                return;
            }

            state.SetPlayers(new List<int>(msg.playerIds));
        });

        clientRouter.Register<StartGameMessage>(MessageType.StartGame, msg =>
        {
            OnStartGameReceived?.Invoke();
        });

        INetworkClient client = new NetworkClient();
        INetworkServer server = new NetworkServer();

        var connectionManager = new ConnectionManager();
        var serverRouter = new ServerMessageRouter(state, connectionManager);

        serverRouter.OnStartGame += () => OnStartGameReceived?.Invoke();

        clientSystem = new ClientSystem(clientRouter, client);
        serverSystem = new ServerSystem(serverRouter, server);

        clientSystem.OnDebug += (msg) => OnDebug?.Invoke(msg);
        serverSystem.OnDebug += (msg) => OnDebug?.Invoke(msg);
    }

    public async Task ConnectToHost(string ip, int port)
    {
        isHost = false;
        await clientSystem.Connect(ip, port);
    }

    public void StartHost(int port)
    {
        isHost = true;
        serverSystem.Start(port);
    }

    public void Send(NetMessage msg)
    {
        MessageWrapper wrapper = new MessageWrapper
        {
            type = GetMessageType(msg),
            json = JsonUtility.ToJson(msg)
        };

        string finalJson = JsonUtility.ToJson(wrapper);

        if (isHost)
        {
            serverSystem.SimulateReceive(finalJson);
        }
        else
        {
            clientSystem.Send(finalJson);
        }
    }

    private MessageType GetMessageType(NetMessage msg)
    {
        if (msg is StartGameMessage) return MessageType.StartGame;
        if (msg is PlayerListMessage) return MessageType.PlayerList;
        if (msg is AssignIdMessage) return MessageType.AssignId;
        if (msg is HelloMessage) return MessageType.Hello;

        throw new Exception("Tipo de mensaje no registrado: " + msg.GetType());
    }
}