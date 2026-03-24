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

    public void ApplyPlayerList(List<int> newPlayers)
    {
        players.Clear();
        players.AddRange(newPlayers);

        OnPlayersUpdated?.Invoke();
    }

}