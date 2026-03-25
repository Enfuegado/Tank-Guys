using System;

public interface ITransport
{
    event Action<NetMessage> OnMessage;
    event Action OnDisconnected;

    void Send(NetMessage message);

    void Start();
    void Stop();
}