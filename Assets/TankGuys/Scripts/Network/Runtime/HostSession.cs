using System;

public class HostSession
{
    private TcpServerRuntime TcpServerRuntime;
    private ServerMessageProcessor serverRouter;
    private ConnectionManager connectionManager;

    public ServerMessageProcessor Router => serverRouter;

    public HostSession()
    {
        connectionManager = new ConnectionManager();

        serverRouter = new ServerMessageProcessor(connectionManager);

        var networkServer = new NetworkServer();

        TcpServerRuntime = new TcpServerRuntime(serverRouter, networkServer);

        WireLogic();
    }

    private void WireLogic()
    {
        serverRouter.OnStartGameRequested += (playerId) =>
        {
            int hostId = serverRouter.GetHostId();

            if (playerId != hostId)
                return;

            serverRouter.Broadcast(new StartGameMessage());
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