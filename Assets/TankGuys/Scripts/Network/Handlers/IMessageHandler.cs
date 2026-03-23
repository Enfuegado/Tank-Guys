public interface IMessageHandler<T> where T : NetMessage
{
    void Handle(T message);
}