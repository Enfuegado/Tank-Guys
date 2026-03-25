using System;

[Serializable]
public class MoveMessage : NetMessage
{
    public int playerId;
    public float x;
    public float y;
}