using System;
using System.Collections.Generic;

public class NetworkState
{
    private readonly List<int> players = new List<int>();

    public IReadOnlyList<int> Players => players;

    public int MyPlayerId { get; private set; } = -1;

    public event Action OnPlayersUpdated;

    public void SetMyPlayerId(int id)
    {
        MyPlayerId = id;
    }

    public void SetPlayers(List<int> newPlayers)
    {
        players.Clear();
        players.AddRange(newPlayers);

        OnPlayersUpdated?.Invoke();
    }

    public void AddPlayer(int id)
    {
        if (!players.Contains(id))
        {
            players.Add(id);
            OnPlayersUpdated?.Invoke();
        }
    }

    public void RemovePlayer(int id)
    {
        if (players.Remove(id))
        {
            OnPlayersUpdated?.Invoke();
        }
    }
}