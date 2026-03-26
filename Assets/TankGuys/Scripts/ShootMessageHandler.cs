public class ShootMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as ShootMessage;
        if (msg == null) return;

        client.Logic.OnPlayerShoot(msg.playerId, msg.dirX, msg.dirY);
    }
}