using System;

public class GameClient
{
    private ITransport transport;
    private GameState state;
    private GameLogic logic;

    public GameState State => state;

    public event Action OnStartGame;
    public event Action OnDisconnected;

    public GameClient(ITransport transport)
    {
        this.transport = transport;

        state = new GameState();
        logic = new GameLogic(state);

        transport.OnMessage += HandleMessage;
        transport.OnDisconnected += HandleDisconnect;
    }

    public void Start()
    {
        transport.Start();
    }

    public void Stop()
    {
        transport.Stop();
    }

    public void Send(NetMessage msg)
    {
        transport.Send(msg);
    }

    private void HandleMessage(NetMessage msg)
    {
        switch (msg)
        {
            case AssignIdMessage m:
                state.LocalPlayerId = m.playerId;
                logic.OnPlayerConnected(m.playerId);
                break;

            case PlayerListMessage m:
                state.SetPlayers(m.playerIds);
                break;

            case MoveMessage m:
                logic.OnPlayerMoved(m.playerId, m.x, m.y);
                break;

            case StartGameMessage m:
                OnStartGame?.Invoke();
                break;
        }
    }

    private void HandleDisconnect()
    {
        OnDisconnected?.Invoke();
    }
}