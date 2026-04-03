using UnityEngine;

public class BannedHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as BannedMessage;
        if (msg == null) return;

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.SetDisconnectReason(msg.reason);

            NetworkBootstrap.Instance.ResetNetwork();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        });
    }
}