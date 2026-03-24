using System;
using System.Threading.Tasks;

public class NetworkManager
{
    private INetworkRole role;

    public NetworkState State => role?.State;

    public event Action OnDisconnected;

    public void StartHost()
    {
        role = new HostNetwork();

        role.OnDisconnected += HandleDisconnect;

        role.Start();
    }

    public async Task Join(string ip, int port)
    {
        role = new ClientNetwork();

        role.OnDisconnected += HandleDisconnect;

        await role.Connect(ip, port);
    }

    public void Send(NetMessage msg)
    {
        role?.Send(msg);
    }

    public void Shutdown()
    {
        if (role != null)
        {
            role.OnDisconnected -= HandleDisconnect;
            role.Shutdown();
        }

        role = null;
    }

    private void HandleDisconnect()
    {
        role?.Shutdown();
        role = null;

        OnDisconnected?.Invoke();
    }

    public void SetHandler(IGameMessageHandler handler)
    {
        if (role is ClientNetwork c) c.SetHandler(handler);
        if (role is HostNetwork h) h.SetHandler(handler);
    }

    public T GetRole<T>() where T : class, INetworkRole
    {
        return role as T;
    }
}