public class AssignIdMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as AssignIdMessage;

        if (msg == null) return;

        client.State.LocalPlayerId = msg.playerId;
        client.Logic.OnPlayerConnected(msg.playerId);
    }
}