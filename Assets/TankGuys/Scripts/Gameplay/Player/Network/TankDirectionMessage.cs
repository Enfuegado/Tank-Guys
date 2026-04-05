using System;

[Serializable]
public class TankDirectionMessage : NetMessage
{
    public int playerId;
    public int direction; 
}