using System;
using UnityEngine;

public class GameClient
{
    private ITransport transport;
    private GameState state;
    private GameLogic logic;

    private MessageDispatcher dispatcher;
    private SnapshotProcessor snapshotProcessor;

    public GameState State => state;
    public GameLogic Logic => logic;

    public event Action OnStartGame;
    public event Action OnDisconnected;

    public GameClient(ITransport transport)
    {
        this.transport = transport;

        state = new GameState();
        logic = new GameLogic(state);

        dispatcher = new MessageDispatcher(this);
        snapshotProcessor = new SnapshotProcessor(state, logic);

        RegisterHandlers();

        transport.OnMessage += HandleMessage;
        transport.OnDisconnected += HandleDisconnect;
    }

    private void RegisterHandlers()
    {
        dispatcher.Register<AssignIdMessage>(new AssignIdHandler());
        dispatcher.Register<PlayerListMessage>(new PlayerListHandler());
        dispatcher.Register<MoveMessage>(new MoveHandler());
        dispatcher.Register<StartGameMessage>(new StartGameHandler());
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
        dispatcher.Dispatch(msg);
    }

    private void HandleDisconnect()
    {
        OnDisconnected?.Invoke();
    }

    public void TriggerStartGame()
    {
        OnStartGame?.Invoke();
    }

    public void ApplySnapshot(GameSnapshot snapshot)
    {
        if (snapshot == null) return;

        snapshotProcessor.Apply(snapshot);

        if (snapshot.gameStarted)
        {
            OnStartGame?.Invoke();
        }
    }
}