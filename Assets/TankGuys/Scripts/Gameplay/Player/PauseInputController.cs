using UnityEngine;

public class PauseInputController : MonoBehaviour
{
    private bool initialized = false;
    private bool isHost = false;
    private bool isPaused = false;

    private GameClient client;

    void Update()
    {
        if (!initialized)
        {
            if (NetworkBootstrap.Instance == null) return;

            client = NetworkBootstrap.Instance.ActiveClient;
            if (client == null) return;

            if (client.State.LocalPlayerId == -1) return;

            initialized = true;
            isHost = NetworkBootstrap.Instance.IsHost;

            if (!isHost)
            {
                enabled = false;
                return;
            }
        }

        if (client.State.Phase == GamePhase.Ended)
            return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            NetworkBootstrap.Instance.Send(new PauseMessage
            {
                isPaused = !isPaused
            });
        }
    }

    public void ApplyPauseState(bool paused)
    {
        isPaused = paused;
    }
}