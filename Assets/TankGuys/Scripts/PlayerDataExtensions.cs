public static class PlayerDataExtensions
{
    public static bool IsAlive(this PlayerData player)
    {
        return player.Status == PlayerStatus.Alive;
    }

    public static bool IsSpectator(this PlayerData player)
    {
        return player.Status == PlayerStatus.Spectator;
    }
}