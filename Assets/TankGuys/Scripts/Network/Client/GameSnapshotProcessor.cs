using System.Collections.Generic;

public class GameSnapshotProcessor
{
    private GameState state;
    private GameLogic logic;

    public GameSnapshotProcessor(GameState state, GameLogic logic)
    {
        this.state = state;
        this.logic = logic;
    }

    public void Apply(GameSnapshot snapshot)
    {
        if (snapshot == null) return;

        HashSet<int> ids = new HashSet<int>();

        foreach (var p in snapshot.players)
        {
            ids.Add(p.id);

            if (!state.Players.ContainsKey(p.id))
                logic.OnPlayerConnected(p.id);

            logic.OnPlayerMoved(p.id, p.x, p.y);
        }

        var toRemove = new List<int>();

        foreach (var id in state.Players.Keys)
        {
            if (!ids.Contains(id))
                toRemove.Add(id);
        }

        foreach (var id in toRemove)
        {
            logic.OnPlayerDisconnected(id);
        }
    }
}