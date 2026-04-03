using System;

[Serializable]
public class BannedMessage : NetMessage
{
    public string reason;
}