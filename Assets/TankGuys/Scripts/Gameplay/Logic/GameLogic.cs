using System;
using System.Linq;
using UnityEngine;

public class GameLogic
{
    private GameState state;

    public event Action<int, float, float> OnShoot;

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
        CheckWinner();
    }

    public void OnPlayerMoved(int id, float x, float y)
    {
        if (!state.Players.TryGetValue(id, out var player))
            return;

        if (player.Status != PlayerStatus.Alive)
            return;

        float speed = 5f;

        Vector2 input = new Vector2(x, y);
        player.Position += input * speed * UnityEngine.Time.deltaTime;
    }

    public void OnPlayerShoot(int id, float dirX, float dirY)
    {
        Debug.Log("CLIENT RECIBE DISPARO de: " + id);
        if (!state.Players.TryGetValue(id, out var player))
            return;

        if (player.Status != PlayerStatus.Alive)
            return;

        OnShoot?.Invoke(id, dirX, dirY);
    }

    public void DamagePlayer(int id)
    {
        if (!state.Players.TryGetValue(id, out var player))
            return;

        if (player.Status != PlayerStatus.Alive)
            return;

        player.Lives--;

        if (player.Lives <= 0)
        {
            player.Status = PlayerStatus.Spectator;
            CheckWinner();
        }
    }

    private void CheckWinner()
    {
        var alive = state.Players.Values
            .Where(p => p.Status == PlayerStatus.Alive)
            .ToList();

        if (alive.Count == 1)
        {
            state.WinnerId = alive[0].Id;
            state.Phase = GamePhase.Ended;
        }
    }
}