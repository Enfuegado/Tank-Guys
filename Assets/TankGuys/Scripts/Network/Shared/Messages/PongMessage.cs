using System;

[Serializable]
public class PongMessage : NetMessage
{
    public int playerId;
    public long timestamp;
}