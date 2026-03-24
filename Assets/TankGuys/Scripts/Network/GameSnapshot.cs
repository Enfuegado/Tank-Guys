using System.Collections.Generic;

[System.Serializable]
public class GameSnapshot
{
    public List<PlayerSnapshot> players;
    public bool gameStarted;
}

[System.Serializable]
public class PlayerSnapshot
{
    public int id;
    public float x;
    public float y;
}