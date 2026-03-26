public class PlayerStateMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as PlayerStateMessage;
        if (msg == null) return;

        if (client.State.Players.TryGetValue(msg.playerId, out var player))
        {
            player.Lives = msg.lives;
            player.Status = (PlayerStatus)msg.status;
        }
    }
}