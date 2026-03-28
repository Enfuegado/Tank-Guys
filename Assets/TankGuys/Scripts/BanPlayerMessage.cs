using System;

[Serializable]
public class BanPlayerMessage : NetMessage
{
    public int targetId;
}