using System;

[Serializable]
public class ConnectionRejectedMessage : NetMessage
{
    public string reason;
}