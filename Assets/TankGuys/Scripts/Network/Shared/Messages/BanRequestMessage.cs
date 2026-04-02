using System;

[Serializable]
public class BanRequestMessage : NetMessage
{
    public int targetId;
}