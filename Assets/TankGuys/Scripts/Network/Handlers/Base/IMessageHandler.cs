public interface IMessageHandler
{
    void Handle(NetMessage message, GameClient client);
}