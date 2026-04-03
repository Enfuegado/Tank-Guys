using System;
using System.Collections.Generic;
using System.Net.Sockets;

public class ServerAdminService
{
    private ConnectionManager connectionManager;
    private HashSet<string> bannedIPs = new HashSet<string>();

    public event Action<TcpClient, int> OnClientDisconnected;
    public event Action OnPlayerListChanged;

    public ServerAdminService(ConnectionManager connectionManager)
    {
        this.connectionManager = connectionManager;
    }

    public bool IsBanned(TcpClient client)
    {
        string ip = GetIP(client);
        return bannedIPs.Contains(ip);
    }

    public void BanClient(int targetId)
    {
        var client = connectionManager.GetClientById(targetId);
        if (client == null) return;

        string ip = GetIP(client);
        bannedIPs.Add(ip);

        DisconnectClient(client, targetId);
    }

    public void KickClient(int targetId)
    {
        var client = connectionManager.GetClientById(targetId);
        if (client == null) return;

        DisconnectClient(client, targetId);
    }

    private void DisconnectClient(TcpClient client, int id)
    {
        System.Threading.Tasks.Task.Delay(100).ContinueWith(_ =>
        {
            try { client.Close(); } catch {}
        });

        connectionManager.RemoveClient(client);

        OnClientDisconnected?.Invoke(client, id);
        OnPlayerListChanged?.Invoke();
    }

    private string GetIP(TcpClient client)
    {
        return ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
    }
}