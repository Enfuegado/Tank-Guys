public class StartGameMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        client.TriggerStartGame();
    }
}