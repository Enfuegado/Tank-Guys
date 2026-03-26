using System;

[Serializable]
public class PlayerStateMessage : NetMessage
{
    public int playerId;
    public int lives;
    public int status;
}