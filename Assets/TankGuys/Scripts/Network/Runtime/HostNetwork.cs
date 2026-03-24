using System;
using System.Threading.Tasks;
using UnityEngine;

public class HostNetwork : INetworkRole
{
    private ClientNetwork client;
    private ServerNetwork server;

    public NetworkState State => client.State;

    public event Action OnDisconnected;

    public HostNetwork()
    {
        server = new ServerNetwork();
        client = new ClientNetwork();

        client.OnDisconnected += () =>
        {
            OnDisconnected?.Invoke();
        };
    }

    public async Task Start()
    {
        server.Start(7777);

        await client.Connect("127.0.0.1", 7777);
    }

    public async Task Connect(string ip, int port)
    {
    }

    public void Send(NetMessage msg)
    {
        client.Send(msg);
    }

    public void Shutdown()
    {
        client.Shutdown();
        server.Stop();
    }

    public void SetHandler(IGameMessageHandler handler)
    {
        client.SetHandler(handler);
    }
}