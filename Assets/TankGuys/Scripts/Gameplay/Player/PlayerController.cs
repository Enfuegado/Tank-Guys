using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform turret;
    private PlayerTag tag;

    private float rotationSpeed = 10f;
    private float turretRotationSpeed = 15f;

    void Start()
    {
        tag = GetComponent<PlayerTag>();

        turret = transform.Find("Turret");

        if (turret == null)
        {
            Debug.LogError("Turret no encontrada en " + gameObject.name);
        }
    }

    void Update()
    {
        var net = NetworkBootstrap.Instance;
        if (net == null) return;

        var client = net.ActiveClient;
        if (client == null) return;

        if (client.State.IsGameplayBlocked())
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

        SmoothTankRotation(player);
        HandleTurret(player);
    }

    private void ApplyRemote(PlayerData player)
    {
        SmoothTankRotation(player);

        if (turret != null)
        {
            Quaternion target = Quaternion.Euler(0, 0, player.TurretRotation);

            turret.rotation = Quaternion.Lerp(
                turret.rotation,
                target,
                turretRotationSpeed * Time.deltaTime
            );
        }
    }

    private void SmoothTankRotation(PlayerData player)
    {
        float angle = player.TankDirection * 45f;

        Quaternion target = Quaternion.Euler(0, 0, angle);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            target,
            rotationSpeed * Time.deltaTime
        );
    }

    private void HandleTurret(PlayerData player)
    {
        if (turret == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorld - turret.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion target = Quaternion.Euler(0, 0, angle);

        turret.rotation = Quaternion.Lerp(
            turret.rotation,
            target,
            turretRotationSpeed * Time.deltaTime
        );

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