using System;

[Serializable]
public class PingMessage : NetMessage
{
    public int playerId;
    public long timestamp;
}