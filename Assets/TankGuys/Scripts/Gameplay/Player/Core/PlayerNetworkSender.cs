using UnityEngine;

public class PlayerNetworkSender
{
    public void SendTankDirection(int playerId, int direction)
    {
        NetworkBootstrap.Instance.Send(new TankDirectionMessage
        {
            playerId = playerId,
            direction = direction
        });
    }

    public void SendTurretRotation(int playerId, float angle)
    {
        NetworkBootstrap.Instance.Send(new TurretRotationMessage
        {
            playerId = playerId,
            angle = angle
        });
    }

    public void SendShoot(int playerId)
    {
        NetworkBootstrap.Instance.Send(new ShootMessage
        {
            playerId = playerId
        });
    }
}