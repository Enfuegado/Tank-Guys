using System;

public interface ITransport
{
    event Action<NetMessage> OnMessage;
    event Action OnDisconnected;

    void Connect(string ip, int port); // 🔥 clave
    void Send(NetMessage message);
    void Stop();
}