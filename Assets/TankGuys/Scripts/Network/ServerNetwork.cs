using System;

public class ServerNetwork
{
    private ServerSystem serverSystem;
    private ServerMessageRouter serverRouter;
    private ConnectionManager connectionManager;

    public ServerMessageRouter Router => serverRouter;

    public ServerNetwork()
    {
        connectionManager = new ConnectionManager();

        serverRouter = new ServerMessageRouter(connectionManager);

        serverSystem = new ServerSystem(serverRouter, new NetworkServer());

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
        serverSystem.Start(port);
    }

    public void Stop()
    {
        serverSystem.Stop();
    }
}