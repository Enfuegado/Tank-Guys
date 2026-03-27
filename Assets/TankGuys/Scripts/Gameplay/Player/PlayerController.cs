using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform turret;

    private PlayerTag tag;

    void Start()
    {
        tag = GetComponent<PlayerTag>();
    }

    void Update()
    {
        var net = NetworkBootstrap.Instance;
        if (net == null) return;

        var client = net.ActiveClient;
        if (client == null) return;

        if (client.State.Phase == GamePhase.Paused)
            return;

        if (!client.State.Players.TryGetValue(tag.PlayerId, out var player))
            return;

        if (tag.PlayerId == client.State.LocalPlayerId)
        {
            HandleLocal(player);
        }
        else
        {
            ApplyRemote(player);
        }
    }

    private void HandleLocal(PlayerData player)
    {
        if (player.Status != PlayerStatus.Alive)
            return;

        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (input.sqrMagnitude > 0.01f)
        {
            int dir = GetDirectionIndex(input);
            player.TankDirection = dir;

            NetworkBootstrap.Instance.Send(new TankDirectionMessage
            {
                playerId = tag.PlayerId,
                direction = dir
            });
        }

        ApplyRotation(player);
        HandleTurret(player);
    }

    private void ApplyRemote(PlayerData player)
    {
        ApplyRotation(player);
        turret.rotation = Quaternion.Euler(0, 0, player.TurretRotation);
    }

    private void ApplyRotation(PlayerData player)
    {
        float angle = player.TankDirection * 45f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void HandleTurret(PlayerData player)
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorld - turret.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        turret.rotation = Quaternion.Euler(0, 0, angle);

        player.TurretRotation = angle;

        NetworkBootstrap.Instance.Send(new TurretRotationMessage
        {
            playerId = tag.PlayerId,
            angle = angle
        });
    }

    private int GetDirectionIndex(Vector2 input)
    {
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

        if (angle < 0) angle += 360f;

        return Mathf.RoundToInt(angle / 45f) % 8;
    }
}