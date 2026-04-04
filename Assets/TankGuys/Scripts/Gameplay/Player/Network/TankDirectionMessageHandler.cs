public class TankDirectionMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as TankDirectionMessage;
        if (msg == null) return;

        if (client.State.Players.TryGetValue(msg.playerId, out var player))
        {
            player.TankDirection = msg.direction;
        }
    }
}