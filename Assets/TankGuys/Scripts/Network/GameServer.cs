using System.Net.Sockets;

public class GameServer
{
    private ServerSystem serverSystem;
    private ServerMessageRouter router;
    private ConnectionManager connectionManager;

    private GameState state;
    private GameLogic logic;

    public GameServer()
    {
        connectionManager = new ConnectionManager();
        router = new ServerMessageRouter(connectionManager);
        serverSystem = new ServerSystem(router, new NetworkServer());

        state = new GameState();
        logic = new GameLogic(state);

        Wire();
    }

    private void Wire()
    {
        router.OnStartGameRequested += (playerId) =>
        {
            int hostId = router.GetHostId();
            if (playerId != hostId) return;

            router.Broadcast(new StartGameMessage());
        };

        router.OnClientConnected += (client, id) =>
        {
            logic.OnPlayerConnected(id);
        };

        router.OnClientDisconnected += (client, id) =>
        {
            logic.OnPlayerDisconnected(id);
        };

        router.OnMoveReceived += (client, msg) =>
        {
            logic.OnPlayerMoved(msg.playerId, msg.x, msg.y);
            router.Broadcast(msg);
        };
    }

    public void Start(int port)
    {
        serverSystem.Start(port);
    }

    public void Stop()
    {
        serverSystem.Stop();
    }
}