using System;

[Serializable]
public class TankDirectionMessage : NetMessage
{
    public int playerId;
    public int direction; // 0-7 (8 direcciones)
}