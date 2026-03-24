using System;

public class NetworkManager
{
    private GameClient client;

    public GameState State => client?.State;

    public event Action OnDisconnected;

    public void StartHost()
    {
        var transport = new TcpTransport();
        client = new GameClient(transport);
        client.Start();
    }

    public void Join(string ip, int port)
    {
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
        client = null;
    }
}