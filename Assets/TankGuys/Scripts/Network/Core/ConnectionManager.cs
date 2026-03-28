using System.Collections.Generic;
using System.Net.Sockets;

public class ConnectionManager
{
    private int nextPlayerId = 1;

    private Dictionary<TcpClient, int> clientIds = new Dictionary<TcpClient, int>();

    private Queue<int> freeIds = new Queue<int>();

    public int RegisterClient(TcpClient client)
    {
        if (clientIds.ContainsKey(client))
            return clientIds[client];

        int id;

        if (freeIds.Count > 0)
        {
            id = freeIds.Dequeue();
        }
        else
        {
            id = nextPlayerId;
            nextPlayerId++;
        }

        clientIds.Add(client, id);

        return id;
    }

    public bool TryGetId(TcpClient client, out int id)
    {
        return clientIds.TryGetValue(client, out id);
    }

    public void RemoveClient(TcpClient client)
    {
        if (clientIds.TryGetValue(client, out int id))
        {
            freeIds.Enqueue(id);
            clientIds.Remove(client);
        }
    }

    public TcpClient GetClientById(int id)
    {
        foreach (var kvp in clientIds)
        {
            if (kvp.Value == id)
                return kvp.Key;
        }
        return null;
    }

    public Dictionary<TcpClient, int> GetAll()
    {
        return clientIds;
    }
}