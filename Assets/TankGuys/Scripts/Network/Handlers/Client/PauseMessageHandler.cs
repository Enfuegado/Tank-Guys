using UnityEngine;

public class PauseMessageHandler : IMessageHandler
{
    public void Handle(NetMessage message, GameClient client)
    {
        var msg = message as PauseMessage;
        if (msg == null) return;

        Time.timeScale = msg.isPaused ? 0f : 1f;

        client.State.Phase = msg.isPaused ? GamePhase.Paused : GamePhase.Playing;

        var ui = GameObject.FindObjectOfType<PauseUIController>();
        if (ui != null)
        {
            ui.ApplyPauseState(msg.isPaused);
        }

        var hostUI = GameObject.FindObjectOfType<HostPauseController>();
        if (hostUI != null)
        {
            hostUI.ApplyPauseState(msg.isPaused);
        }

        var input = GameObject.FindObjectOfType<PauseInputController>();
        if (input != null)
        {
            input.ApplyPauseState(msg.isPaused);
        }
    }
}