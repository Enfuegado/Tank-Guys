using UnityEngine;

public class PlayerData
{
    public int Id;
    public Vector2 Position;

    public int Lives;
    public PlayerStatus Status;

    public PlayerData(int id)
    {
        Id = id;
        Position = Vector2.zero;
        Lives = 5;
        Status = PlayerStatus.Alive;
    }
}