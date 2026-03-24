using System;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSystem
{
    private INetworkClient client;
    private MessageRouter router;

    public Action<string> OnDebug;
    public Action OnDisconnected;

    public ClientSystem(MessageRouter router, INetworkClient client)
    {
        this.router = router;
        this.client = client;
    }

    public async Task Connect(string ip, int port)
    {
        client.OnMessageReceived = null;
        client.OnDisconnected = null;

        client.OnMessageReceived += OnMessageReceived;
        client.OnDisconnected += HandleDisconnect;

        await client.Connect(ip, port);

        OnDebug?.Invoke("CLIENTE CONECTADO");

        HelloMessage hello = new HelloMessage();

        MessageWrapper wrapper = new MessageWrapper
        {
            type = MessageType.Hello,
            json = JsonUtility.ToJson(hello)
        };

        string json = JsonUtility.ToJson(wrapper);

        await client.Send(json);
    }

    private void OnMessageReceived(string json)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            OnDebug?.Invoke("CLIENT RECIBE: " + json);
            router.Handle(json);
        });
    }

    private void HandleDisconnect()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            OnDisconnected?.Invoke();
        });
    }

    public async void Send(string json)
    {
        if (client == null || !client.IsConnected)
        {
            OnDebug?.Invoke("CLIENTE NO CONECTADO");
            return;
        }

        await client.Send(json);
    }

    public void Disconnect()
    {
        client?.Disconnect();
    }
}