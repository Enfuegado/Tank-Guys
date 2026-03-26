public class TurretRotationMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as TurretRotationMessage;
        if (msg == null) return;

        if (client.State.Players.TryGetValue(msg.playerId, out var player))
        {
            player.TurretRotation = msg.angle;
        }
    }
}