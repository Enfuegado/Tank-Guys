using System;
using System.Net.Sockets;

public class ServerConnectionService
{
    private ConnectionManager connectionManager;
    private GameState gameState;

    public event Action<TcpClient, int> OnClientConnected;

    public ServerConnectionService(ConnectionManager connectionManager, GameState gameState)
    {
        this.connectionManager = connectionManager;
        this.gameState = gameState;
    }

    public bool TryHandleConnection(TcpClient sender, Func<TcpClient, bool> isBanned, Action<TcpClient, NetMessage> send, Action broadcastPlayers, ref int hostPlayerId)
    {
        if (isBanned(sender))
        {
            send(sender, new ConnectionRejectedMessage
            {
                reason = "You have been banned by the host"
            });

            Close(sender);
            return false;
        }

        bool matchInProgress = gameState.Phase == GamePhase.Playing || gameState.Phase == GamePhase.Paused;
        bool matchFinished = gameState.Phase == GamePhase.Ended && gameState.Players.Count > 1;

        if (matchInProgress || matchFinished)
        {
            send(sender, new ConnectionRejectedMessage
            {
                reason = "The match has already started"
            });

            Close(sender);
            return false;
        }

        if (connectionManager.GetAll().Count >= 4)
        {
            send(sender, new ConnectionRejectedMessage
            {
                reason = "The maximum number of players has been reached"
            });

            Close(sender);
            return false;
        }

        int id = connectionManager.RegisterClient(sender);

        if (hostPlayerId == -1)
            hostPlayerId = id;

        send(sender, new AssignIdMessage
        {
            playerId = id
        });

        OnClientConnected?.Invoke(sender, id);

        broadcastPlayers();

        return true;
    }

    private void Close(TcpClient client)
    {
        System.Threading.Tasks.Task.Delay(100).ContinueWith(_ =>
        {
            try { client.Close(); } catch {}
        });
    }
}