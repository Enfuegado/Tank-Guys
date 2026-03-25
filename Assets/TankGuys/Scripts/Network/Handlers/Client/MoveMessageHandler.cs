public class MoveMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as MoveMessage;

        if (msg == null) return;

        client.Logic.OnPlayerMoved(msg.playerId, msg.x, msg.y);
    }
}