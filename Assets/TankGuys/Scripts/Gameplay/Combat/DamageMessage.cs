using System;

[Serializable]
public class DamageMessage : NetMessage
{
    public int targetId;
}