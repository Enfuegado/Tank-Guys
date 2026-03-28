using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        var incoming = new HashSet<int>(ids);

        var toRemove = Players.Keys.Where(id => !incoming.Contains(id)).ToList();

        foreach (var id in toRemove)
        {
            Players.Remove(id);
        }

        foreach (var id in incoming)
        {
            if (!Players.ContainsKey(id))
            {
                Players[id] = new PlayerData(id);
            }
        }
    }
}