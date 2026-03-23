using System;

[Serializable]
public class PlayerJoinedMessage : NetMessage
{
    public int playerId;
}