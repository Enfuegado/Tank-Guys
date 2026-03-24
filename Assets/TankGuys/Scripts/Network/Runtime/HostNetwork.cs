public class HostNetwork
{
    private GameServer server;
    private GameClient client;

    public GameClient Client => client;
    public GameState State => client?.State;

    public void StartHost()
    {
        server = new GameServer();
        server.Start(7777);

        var transport = new TcpTransport();
        client = new GameClient(transport);
        client.Start();
    }

    public void Send(NetMessage msg)
    {
        client?.Send(msg);
    }

    public void Shutdown()
    {
        client?.Stop();
        server?.Stop();

        client = null;
        server = null;
    }
}