using System;
using System.Net.Sockets;

public class TcpServerRuntime
{
    private INetworkServer server;
    private ServerMessageProcessor router;

    public Action<string> OnDebug;

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
        catch (Exception)
        {
            OnDebug?.Invoke("ERROR AL INICIAR HOST");
        }

        router.Initialize(server);

        OnDebug?.Invoke("HOST INICIADO");
    }

    private void OnMessageReceived(string json, TcpClient sender)
    {
        try
        {
            router.Handle(json, sender);
        }
        catch (Exception)
        {
            OnDebug?.Invoke("ERROR PROCESANDO MENSAJE");
        }
    }

    private void OnClientDisconnected(TcpClient client)
    {
        try
        {
            router.HandleDisconnect(client);
        }
        catch (Exception)
        {
            OnDebug?.Invoke("ERROR EN DESCONEXIÓN");
        }
    }

    public async void Broadcast(string json)
    {
        try
        {
            await server.Broadcast(json);
        }
        catch (Exception)
        {
            OnDebug?.Invoke("ERROR EN BROADCAST");
        }
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