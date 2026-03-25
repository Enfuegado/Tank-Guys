public class PlayerListHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as PlayerListMessage;

        if (msg == null) return;

        client.State.SetPlayers(msg.playerIds);
    }
}