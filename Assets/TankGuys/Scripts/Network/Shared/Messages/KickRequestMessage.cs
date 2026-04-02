using System;

[Serializable]
public class KickRequestMessage : NetMessage
{
    public int targetId;
}