using System;

[Serializable]
public class PingReportMessage : NetMessage
{
    public int playerId;
    public int ping;
}