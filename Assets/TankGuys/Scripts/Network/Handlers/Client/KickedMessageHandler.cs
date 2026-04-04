using UnityEngine;

public class KickedMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as KickedMessage;
        if (msg == null) return;

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.SetDisconnectReason(msg.reason);

            NetworkBootstrap.Instance.ResetNetwork();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        });
    }
}