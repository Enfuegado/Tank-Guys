using System;
using System.Threading.Tasks;
using UnityEngine;

public class TcpClientRuntime
{
    private INetworkClient client;
    private MessageRouter router;

    public Action<string> OnDebug;
    public Action OnDisconnected;

    private bool isDisconnecting = false;
    private bool receivedRejection = false;

    private string sessionId;

    public TcpClientRuntime(MessageRouter router, INetworkClient client)
    {
        this.router = router;
        this.client = client;
        sessionId = Guid.NewGuid().ToString();
    }

    public async Task Connect(string ip, int port)
    {
        try
        {
            client.OnMessageReceived = null;
            client.OnDisconnected = null;

            client.OnMessageReceived += OnMessageReceived;
            client.OnDisconnected += HandleDisconnect;

            await client.Connect(ip, port);

            OnDebug?.Invoke("CLIENTE CONECTADO");

            HelloMessage hello = new HelloMessage
            {
                sessionId = sessionId
            };

            MessageWrapper wrapper = new MessageWrapper
            {
                type = MessageType.Hello,
                json = JsonUtility.ToJson(hello)
            };

            string json = JsonUtility.ToJson(wrapper);

            await client.Send(json);
        }
        catch (Exception)
        {
            HandleDisconnect();
        }
    }

    private void OnMessageReceived(string json)
    {
        if (json.Contains("ConnectionRejected"))
        {
            receivedRejection = true;
        }

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            OnDebug?.Invoke("CLIENT RECIBE: " + json);
            router.Handle(json);
        });
    }

    private void HandleDisconnect()
    {
        if (isDisconnecting) return;
        isDisconnecting = true;

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            OnDebug?.Invoke("CLIENTE DESCONECTADO");

            if (receivedRejection)
                return;

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

        try
        {
            await client.Send(json);
        }
        catch (Exception)
        {
            HandleDisconnect();
        }
    }

    public void Disconnect()
    {
        try
        {
            client?.Disconnect();
        }
        catch { }

        HandleDisconnect();
    }
}