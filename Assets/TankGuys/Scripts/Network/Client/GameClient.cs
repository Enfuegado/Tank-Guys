using System;

public class GameClient
{
    private ITransport transport;
    private GameState state;
    private GameLogic logic;

    private MessageDispatcher dispatcher;
    private GameSnapshotProcessor snapshotProcessor;

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
        snapshotProcessor = new GameSnapshotProcessor(state, logic);

        RegisterHandlers();

        transport.OnMessage += HandleMessage;
        transport.OnDisconnected += HandleDisconnect;
    }

    private void RegisterHandlers()
    {
        dispatcher.Register<AssignIdMessage>(new AssignIdMessageHandler());
        dispatcher.Register<PlayerListMessage>(new PlayerListMessageHandler());
        dispatcher.Register<MoveMessage>(new MoveMessageHandler());
        dispatcher.Register<StartGameMessage>(new StartGameMessageHandler());
        dispatcher.Register<ShootMessage>(new ShootMessageHandler());
        dispatcher.Register<PlayerStateMessage>(new PlayerStateMessageHandler());
        dispatcher.Register<TurretRotationMessage>(new TurretRotationMessageHandler());
        dispatcher.Register<TankDirectionMessage>(new TankDirectionMessageHandler());
        dispatcher.Register<PauseMessage>(new PauseMessageHandler());
        dispatcher.Register<GameEndMessage>(new GameEndMessageHandler());
        dispatcher.Register<ConnectionRejectedMessage>(new ConnectionRejectedHandler());
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