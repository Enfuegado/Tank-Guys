using System;
using System.Collections.Generic;

public class PlayerRegistry
{
    private HashSet<int> players = new HashSet<int>();

    public IReadOnlyCollection<int> Players => players;

    public Action<int> OnPlayerAdded;
    public Action<int> OnPlayerRemoved;

    public void Sync(IEnumerable<int> incoming)
    {
        HashSet<int> newSet = new HashSet<int>(incoming);

        foreach (var id in newSet)
        {
            if (!players.Contains(id))
            {
                players.Add(id);
                OnPlayerAdded?.Invoke(id);
            }
        }

        foreach (var id in new List<int>(players))
        {
            if (!newSet.Contains(id))
            {
                players.Remove(id);
                OnPlayerRemoved?.Invoke(id);
            }
        }
    }
}