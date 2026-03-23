using System;
using System.Collections.Generic;

public class NetworkState
{
    private List<int> players = new List<int>();

    public IReadOnlyList<int> Players => players;

    public int myPlayerId = -1;

    public Action OnPlayersUpdated;

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