public class PongMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as PongMessage;
        if (msg == null) return;

        if (PingSystem.Instance != null)
        {
            PingSystem.Instance.HandlePong(msg);
        }
    }
}