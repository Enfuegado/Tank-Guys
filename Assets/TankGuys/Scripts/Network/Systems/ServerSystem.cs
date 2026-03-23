using System;
using System.Net.Sockets;

public class ServerSystem
{
    private INetworkServer server;
    private ServerMessageRouter router;

    public Action<string> OnDebug;

    public ServerSystem(ServerMessageRouter router, INetworkServer server)
    {
        this.router = router;
        this.server = server;
    }

    public void Start(int port)
    {
        server.OnMessageReceived += OnMessageReceived;
        server.OnClientDisconnected += OnClientDisconnected;

        _ = server.Start(port);

        router.Initialize(server);

        OnDebug?.Invoke("HOST INICIADO");
    }

    private void OnMessageReceived(string json, TcpClient sender)
    {
        OnDebug?.Invoke("SERVER RECIBE: " + json);
        router.Handle(json, sender);
    }

    private void OnClientDisconnected(TcpClient client)
    {
        router.HandleDisconnect(client);
    }

    public async void Broadcast(string json)
    {
        await server.Broadcast(json);
    }

    public void SimulateReceive(string json)
    {
        router.Handle(json, null);
    }
}