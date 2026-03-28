using UnityEngine;

public class ConnectionRejectedHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as ConnectionRejectedMessage;
        if (msg == null) return;

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (ErrorPanelUI.Instance != null)
            {
                ErrorPanelUI.Instance.Show(msg.reason);
            }

            var menu = GameObject.FindObjectOfType<MainMenuUI>();
            if (menu != null)
            {
                menu.ResetUI();
            }

            NetworkBootstrap.Instance.ResetNetwork();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.MarkConnectionRejected();
            }
        });
    }
}