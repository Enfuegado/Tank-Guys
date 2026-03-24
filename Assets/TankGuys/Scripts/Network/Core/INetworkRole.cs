using System;
using System.Threading.Tasks;

public interface INetworkRole
{
    NetworkState State { get; }

    event Action OnDisconnected;

    Task Start();
    Task Connect(string ip, int port);
    void Send(NetMessage msg);
    void Shutdown();
}