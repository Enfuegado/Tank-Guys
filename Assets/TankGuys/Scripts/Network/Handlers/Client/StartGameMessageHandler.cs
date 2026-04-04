public class StartGameMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        client.State.Phase = GamePhase.Playing;
        client.TriggerStartGame();
    }
}