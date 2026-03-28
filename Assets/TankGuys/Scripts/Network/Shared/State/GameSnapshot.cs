using System.Collections.Generic;

[System.Serializable]
public class GameSnapshot
{
    public List<PlayerSnapshot> players;
    public bool gameStarted;
}
