using System.Net.Sockets;

public class GameServer
{
    private TcpServerRuntime TcpServerRuntime;
    private ServerMessageProcessor router;
    private ConnectionManager connectionManager;

    private GameState state;
    private GameLogic logic;

    public GameServer()
    {
        connectionManager = new ConnectionManager();
        router = new ServerMessageProcessor(connectionManager);
        TcpServerRuntime = new TcpServerRuntime(router, new NetworkServer());

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

        router.OnShootReceived += (client, msg) =>
        {
            logic.OnPlayerShoot(msg.playerId, msg.dirX, msg.dirY);
            router.Broadcast(msg);
        };

        router.OnDamageReceived += (client, msg) =>
        {
            logic.DamagePlayer(msg.targetId);

            var player = state.Players[msg.targetId];

            router.Broadcast(new PlayerStateMessage
            {
                playerId = player.Id,
                lives = player.Lives,
                status = (int)player.Status
            });
        };
        router.OnTurretRotationReceived += (client, msg) =>
        {
            router.Broadcast(msg);
        };
        router.OnTankDirectionReceived += (client, msg) =>
        {
            router.Broadcast(msg);
        };
    }

    public void Start(int port)
    {
        TcpServerRuntime.Start(port);
    }

    public void Stop()
    {
        TcpServerRuntime.Stop();
    }
}