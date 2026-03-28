using System;

[Serializable]
public class KickPlayerMessage : NetMessage
{
    public int targetId;
}