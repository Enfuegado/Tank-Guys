using System;
using System.Net.Sockets;

public class TcpServerRuntime
{
    private INetworkServer server;
    private ServerMessageProcessor router;

    public TcpServerRuntime(ServerMessageProcessor router, INetworkServer server)
    {
        this.router = router;
        this.server = server;
    }

    public void Start(int port)
    {
        server.OnMessageReceived -= OnMessageReceived;
        server.OnMessageReceived += OnMessageReceived;

        server.OnClientDisconnected -= OnClientDisconnected;
        server.OnClientDisconnected += OnClientDisconnected;

        try
        {
            _ = server.Start(port);
        }
        catch (Exception) { }

        router.Initialize(server);
    }

    private void OnMessageReceived(string json, TcpClient sender)
    {
        try
        {
            router.Handle(json, sender);
        }
        catch (Exception) { }
    }

    private void OnClientDisconnected(TcpClient client)
    {
        try
        {
            router.HandleDisconnect(client);
        }
        catch (Exception) { }
    }

    public async void Broadcast(string json)
    {
        try
        {
            await server.Broadcast(json);
        }
        catch (Exception) { }
    }

    public void Stop()
    {
        try
        {
            server?.Stop();
        }
        catch { }
    }
}