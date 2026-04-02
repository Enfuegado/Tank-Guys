using UnityEngine;

public class BannedHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as BannedMessage;
        if (msg == null) return;

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (ErrorPanelUI.Instance != null)
            {
                ErrorPanelUI.Instance.Show(msg.reason);
            }

            System.Threading.Tasks.Task.Delay(200).ContinueWith(_ =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (NetworkBootstrap.Instance != null)
                    {
                        NetworkBootstrap.Instance.ResetNetwork();
                    }
                });
            });
        });
    }
}