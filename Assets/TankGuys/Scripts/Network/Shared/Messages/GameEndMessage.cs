using System;

[Serializable]
public class GameEndMessage : NetMessage
{
    public int winnerId;
}