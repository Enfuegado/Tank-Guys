using UnityEngine;

public class PlayerLocalController : MonoBehaviour
{
    private Transform turret;
    private PlayerTag tag;

    private float rotationSpeed = 10f;
    private float turretRotationSpeed = 15f;

    private PlayerNetworkSender networkSender = new PlayerNetworkSender();

    void Start()
    {
        tag = GetComponent<PlayerTag>();
        turret = transform.Find("Turret");
    }

    public void Tick(PlayerData player)
    {
        if (player.Status != PlayerStatus.Alive)
            return;

        HandleMovement(player);
        HandleTurret(player);
    }

    private void HandleMovement(PlayerData player)
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (input.sqrMagnitude > 0.01f)
        {
            int dir = GetDirectionIndex(input);
            player.TankDirection = dir;

            networkSender.SendTankDirection(tag.PlayerId, dir);
        }

        SmoothTankRotation(player);
    }

    private void SmoothTankRotation(PlayerData player)
    {
        float angle = player.TankDirection * 45f;

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0, 0, angle),
            rotationSpeed * Time.deltaTime
        );
    }

    private void HandleTurret(PlayerData player)
    {
        if (turret == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(cam.transform.position.z);

        Vector3 mouseWorld = cam.ScreenToWorldPoint(mousePos);
        Vector2 dir = mouseWorld - turret.position;

        if (dir.sqrMagnitude < 0.001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        turret.rotation = Quaternion.Lerp(
            turret.rotation,
            Quaternion.Euler(0, 0, angle),
            turretRotationSpeed * Time.deltaTime
        );

        player.TurretRotation = angle;

        networkSender.SendTurretRotation(tag.PlayerId, angle);
    }

    private int GetDirectionIndex(Vector2 input)
    {
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

        if (angle < 0) angle += 360f;

        return Mathf.RoundToInt(angle / 45f) % 8;
    }
}