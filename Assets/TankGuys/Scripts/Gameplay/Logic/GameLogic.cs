public class GameLogic
{
    private GameState state;

    public GameLogic(GameState state)
    {
        this.state = state;
    }

    public void OnPlayerConnected(int id)
    {
        state.AddPlayer(id);
    }

    public void OnPlayerDisconnected(int id)
    {
        state.RemovePlayer(id);
    }

    public void OnPlayerMoved(int id, float x, float y)
    {
        state.UpdatePosition(id, new UnityEngine.Vector2(x, y));
    }
}