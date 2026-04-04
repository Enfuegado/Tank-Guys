using System;

[Serializable]
public class PlayerListMessage : NetMessage
{
    public int[] playerIds;
}