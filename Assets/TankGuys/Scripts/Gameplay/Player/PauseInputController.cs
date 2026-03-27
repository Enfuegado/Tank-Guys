using UnityEngine;

public class PauseInputController : MonoBehaviour
{
    private bool initialized = false;
    private bool isHost = false;
    private bool isPaused = false;

    void Update()
    {
        if (!initialized)
        {
            if (NetworkBootstrap.Instance == null) return;

            var client = NetworkBootstrap.Instance.ActiveClient;
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

        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        NetworkBootstrap.Instance.Send(new PauseMessage
        {
            isPaused = isPaused
        });
    }

    public void ApplyPauseState(bool paused)
    {
        isPaused = paused;
    }
}