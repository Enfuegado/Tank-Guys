using System.Net.Sockets;

public interface IServerMessageHandler<T> where T : NetMessage
{
    void Handle(T message, TcpClient sender);
}