using UnityEngine;

public class PlayerRemoteController : MonoBehaviour
{
    private Transform turret;

    private float rotationSpeed = 10f;
    private float turretRotationSpeed = 15f;

    void Start()
    {
        turret = transform.Find("Turret");
    }

    public void Tick(PlayerData player)
    {
        SmoothTankRotation(player);
        SmoothTurretRotation(player);
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

    private void SmoothTurretRotation(PlayerData player)
    {
        if (turret == null) return;

        Quaternion target = Quaternion.Euler(0, 0, player.TurretRotation);

        turret.rotation = Quaternion.Lerp(
            turret.rotation,
            target,
            turretRotationSpeed * Time.deltaTime
        );
    }
}