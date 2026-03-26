using System;

[Serializable]
public class TurretRotationMessage : NetMessage
{
    public int playerId;
    public float angle;
}