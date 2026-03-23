using System;

[Serializable]
public class PlayerLeftMessage : NetMessage
{
    public int playerId;
}