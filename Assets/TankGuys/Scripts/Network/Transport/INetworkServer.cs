using System;
using System.Net.Sockets;
using System.Threading.Tasks;

public interface INetworkServer
{
    Action<string, TcpClient> OnMessageReceived { get; set; }
    Action<TcpClient> OnClientDisconnected { get; set; }

    Task Start(int port);
    Task Send(TcpClient client, string message);
    Task Broadcast(string message);

    void Stop();
}