using System.Net;
using System.Net.Sockets;

public class HostNetwork
{
    private GameServer server;
    private GameClient client;

    public GameClient Client => client;
    public GameState State => client?.State;

    public bool StartHost()
    {
        if (!IsPortAvailable(7777))
            return false;

        server = new GameServer();
        server.Start(7777);

        var transport = new TcpTransport();
        client = new GameClient(transport);
        client.Start();

        return true;
    }

    private bool IsPortAvailable(int port)
    {
        TcpListener listener = null;

        try
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
        finally
        {
            listener?.Stop();
        }
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