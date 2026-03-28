using UnityEngine;
using System;
using System.Collections.Generic;

public class PingSystem : MonoBehaviour
{
    public static PingSystem Instance;

    private GameClient client;

    private Dictionary<long, long> sentTimes = new();
    private Dictionary<int, int> playerPings = new();

    private float timer = 0f;
    private float interval = 1f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (NetworkBootstrap.Instance == null) return;

        client = NetworkBootstrap.Instance.ActiveClient;
        if (client == null) return;

        if (client.State.LocalPlayerId == -1) return;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            SendPing();
        }
    }

    void SendPing()
    {
        long timestamp = DateTime.UtcNow.Ticks;

        sentTimes[timestamp] = timestamp;

        NetworkBootstrap.Instance.Send(new PingMessage
        {
            playerId = client.State.LocalPlayerId,
            timestamp = timestamp
        });
    }

    public void HandlePong(PongMessage msg)
    {
        if (!sentTimes.ContainsKey(msg.timestamp))
            return;

        long sent = sentTimes[msg.timestamp];
        sentTimes.Remove(msg.timestamp);

        long now = DateTime.UtcNow.Ticks;

        long diff = now - sent;

        int ms = (int)(diff / TimeSpan.TicksPerMillisecond);

        playerPings[msg.playerId] = ms;

        NetworkBootstrap.Instance.Send(new PingReportMessage
        {
            playerId = msg.playerId,
            ping = ms
        });
    }

    public void HandlePingReport(PingReportMessage msg)
    {
        playerPings[msg.playerId] = msg.ping;
    }

    public int GetPing(int playerId)
    {
        if (playerPings.TryGetValue(playerId, out int ping))
            return ping;

        return -1;
    }
}