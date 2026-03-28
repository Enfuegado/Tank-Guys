using System;
using System.Net.Sockets;

public class GameServer
{
    private TcpServerRuntime TcpServerRuntime;
    private ServerMessageProcessor router;
    private ConnectionManager connectionManager;

    private GameState state;
    private GameLogic logic;

    private bool gameEndedSent = false;

    public GameServer()
    {
        connectionManager = new ConnectionManager();

        var networkServer = new NetworkServer();

        router = new ServerMessageProcessor(connectionManager);
        router.Initialize(networkServer);

        TcpServerRuntime = new TcpServerRuntime(router, networkServer);

        state = new GameState();
        logic = new GameLogic(state);

        gameEndedSent = false;

        Wire();
    }

    private void Wire()
    {
        router.OnStartGameRequested += (playerId) =>
        {
            int hostId = router.GetHostId();

            if (playerId != hostId)
                return;

            if (state.Players.Count < 2)
                return;

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

            if (!gameEndedSent && state.Phase == GamePhase.Ended && state.WinnerId.HasValue)
            {
                gameEndedSent = true;

                router.Broadcast(new GameEndMessage
                {
                    winnerId = state.WinnerId.Value
                });
            }
        };

        router.OnTurretRotationReceived += (client, msg) =>
        {
            router.Broadcast(msg);
        };

        router.OnTankDirectionReceived += (client, msg) =>
        {
            router.Broadcast(msg);
        };

        router.OnPauseReceived += (client, msg) =>
        {
            if (!connectionManager.TryGetId(client, out int id))
                return;

            int hostId = router.GetHostId();

            if (id != hostId)
                return;

            if (state.Phase == GamePhase.Ended)
                return;

            state.Phase = msg.isPaused ? GamePhase.Paused : GamePhase.Playing;

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