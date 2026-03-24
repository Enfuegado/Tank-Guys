using System.Collections.Generic;
using System.Net.Sockets;

public class ConnectionManager
{
    private int nextPlayerId = 1;
    private Dictionary<TcpClient, int> clientIds = new Dictionary<TcpClient, int>();

    public int RegisterClient(TcpClient client)
    {
        if (clientIds.ContainsKey(client))
            return clientIds[client];

        int id = nextPlayerId;
        nextPlayerId++;

        clientIds.Add(client, id);

        return id;
    }

    public bool TryGetId(TcpClient client, out int id)
    {
        return clientIds.TryGetValue(client, out id);
    }

    public void RemoveClient(TcpClient client)
    {
        if (clientIds.ContainsKey(client))
            clientIds.Remove(client);
    }
}