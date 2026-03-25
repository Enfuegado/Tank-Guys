public class StartGameHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        client.TriggerStartGame();
    }
}