using System;
using System.Collections.Generic;
using UnityEngine;

public class GameClient
{
    private ITransport transport;
    private GameState state;
    private GameLogic logic;

    private Dictionary<Type, IMessageHandler> handlers = new();

    public GameState State => state;
    public GameLogic Logic => logic;

    public event Action OnStartGame;
    public event Action OnDisconnected;

    public GameClient(ITransport transport)
    {
        this.transport = transport;

        state = new GameState();
        logic = new GameLogic(state);

        RegisterHandlers();

        transport.OnMessage += HandleMessage;
        transport.OnDisconnected += HandleDisconnect;
    }

    private void RegisterHandlers()
    {
        handlers[typeof(AssignIdMessage)] = new AssignIdHandler();
        handlers[typeof(PlayerListMessage)] = new PlayerListHandler();
        handlers[typeof(MoveMessage)] = new MoveHandler();
        handlers[typeof(StartGameMessage)] = new StartGameHandler();
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
        var type = msg.GetType();

        if (handlers.TryGetValue(type, out var handler))
        {
            handler.Handle(msg, this);
        }
        else
        {
            Debug.LogWarning("No handler for message: " + type);
        }
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

        HashSet<int> ids = new HashSet<int>();

        foreach (var p in snapshot.players)
        {
            ids.Add(p.id);

            if (!state.Players.ContainsKey(p.id))
                logic.OnPlayerConnected(p.id);

            logic.OnPlayerMoved(p.id, p.x, p.y);
        }

        var toRemove = new List<int>();

        foreach (var id in state.Players.Keys)
        {
            if (!ids.Contains(id))
                toRemove.Add(id);
        }

        foreach (var id in toRemove)
        {
            logic.OnPlayerDisconnected(id);
        }

        if (snapshot.gameStarted)
        {
            OnStartGame?.Invoke();
        }
    }
}