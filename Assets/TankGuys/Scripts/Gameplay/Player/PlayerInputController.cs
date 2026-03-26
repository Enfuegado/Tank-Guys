using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    private GameClient client;
    private int playerId;
    private bool initialized = false;

    void Update()
    {
        if (!initialized)
        {
            if (NetworkBootstrap.Instance == null) return;
            if (NetworkBootstrap.Instance.ActiveClient == null) return;

            client = NetworkBootstrap.Instance.ActiveClient;

            if (client.State.LocalPlayerId == -1) return;

            playerId = client.State.LocalPlayerId;
            initialized = true;
        }

        if (!client.State.Players.TryGetValue(playerId, out var player))
            return;

        if (player.Status != PlayerStatus.Alive)
            return;

        HandleMovement();
        HandleShooting(player);
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        client.Send(new MoveMessage
        {
            playerId = playerId,
            x = x,
            y = y
        });
    }

    private void HandleShooting(PlayerData player)
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 dir = (mouseWorld - new Vector3(player.Position.x, player.Position.y, 0)).normalized;

        client.Send(new ShootMessage
        {
            playerId = playerId,
            dirX = dir.x,
            dirY = dir.y
        });
    }
}