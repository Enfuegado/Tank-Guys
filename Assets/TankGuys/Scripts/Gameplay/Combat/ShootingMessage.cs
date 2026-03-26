using System;

[Serializable]
public class ShootMessage : NetMessage
{
    public int playerId;
    public float dirX;
    public float dirY;
}