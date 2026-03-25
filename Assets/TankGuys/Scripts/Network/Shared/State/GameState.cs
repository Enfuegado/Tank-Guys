using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public Dictionary<int, PlayerData> Players = new();

    public int LocalPlayerId = -1;

    public void AddPlayer(int id)
    {
        if (!Players.ContainsKey(id))
        {
            Players[id] = new PlayerData { Id = id };
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

public class PlayerData
{
    public int Id;
    public Vector2 Position;
}