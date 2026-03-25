using System;
using System.Threading.Tasks;

public interface INetworkClient
{
    bool IsConnected { get; }

    Action<string> OnMessageReceived { get; set; }
    Action OnDisconnected { get; set; }

    Task Connect(string ip, int port);
    Task Send(string message);
    void Disconnect();
}