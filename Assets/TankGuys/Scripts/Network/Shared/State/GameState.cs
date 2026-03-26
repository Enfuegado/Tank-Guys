using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public Dictionary<int, PlayerData> Players = new();

    public int LocalPlayerId = -1;

    public GamePhase Phase = GamePhase.Lobby;

    public int? WinnerId = null;

    public void AddPlayer(int id)
    {
        if (!Players.ContainsKey(id))
        {
            Players[id] = new PlayerData(id);
        }
    }

    public void RemovePlayer(int id)
    {
        if (Players.ContainsKey(id))
        {
            Players.Remove(id);
        }
    }

    public void UpdatePosition(int id, Vector2 position)
    {
        if (Players.TryGetValue(id, out var player))
        {
            player.Position = position;
        }
    }

    public void SetPlayers(IEnumerable<int> ids)
    {
        var newSet = new HashSet<int>(ids);

        foreach (var id in newSet)
        {
            if (!Players.ContainsKey(id))
                AddPlayer(id);
        }

        var toRemove = new List<int>();

        foreach (var id in Players.Keys)
        {
            if (!newSet.Contains(id))
                toRemove.Add(id);
        }

        foreach (var id in toRemove)
        {
            RemovePlayer(id);
        }
    }
}