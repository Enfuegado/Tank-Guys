using System.Linq;

public static class GameStateExtensions
{
    public static int AliveCount(this GameState state)
    {
        return state.Players.Values.Count(p => p.Status == PlayerStatus.Alive);
    }

    public static bool IsGameplayBlocked(this GameState state)
    {
        return state.Phase == GamePhase.Paused || state.Phase == GamePhase.Ended;
    }
}