public class PingReportMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as PingReportMessage;
        if (msg == null) return;

        if (PingSystem.Instance != null)
        {
            PingSystem.Instance.HandlePingReport(msg);
        }
    }
}