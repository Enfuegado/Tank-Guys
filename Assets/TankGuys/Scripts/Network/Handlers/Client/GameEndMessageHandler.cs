using UnityEngine;

public class GameEndMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as GameEndMessage;
        if (msg == null) return;

        client.State.Phase = GamePhase.Ended;
        client.State.WinnerId = msg.winnerId;

        Time.timeScale = 0f;

        var ui = GameObject.FindObjectOfType<GameEndUIController>();
        if (ui != null)
        {
            ui.ShowWinner(msg.winnerId);
        }
    }
}