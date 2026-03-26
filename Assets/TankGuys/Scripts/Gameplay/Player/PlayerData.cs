using UnityEngine;

public class PlayerData
{
    public int Id;

    public Vector2 Position;

    public int Lives;

    public PlayerStatus Status;

    public float TurretRotation;

    public int TankDirection;

    public PlayerData(int id)
    {
        Id = id;

        Position = Vector2.zero;

        Lives = 5;

        Status = PlayerStatus.Alive;

        TurretRotation = 0f;

        TankDirection = 0;
    }
}