using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public GameObject playerPrefab;

    private Dictionary<int, GameObject> playerObjects = new();

    private GameState state;
    private SpawnManager spawnManager;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Initialize(GameState gameState)
    {
        state = gameState;
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    void Update()
    {
        if (state == null) return;

        SyncPlayers();
    }

    private void SyncPlayers()
    {
        foreach (var kvp in state.Players)
        {
            int id = kvp.Key;

            if (!playerObjects.ContainsKey(id))
            {
                SpawnPlayer(id);
            }

            UpdatePlayer(kvp.Value);
        }

        var toRemove = new List<int>();

        foreach (var id in playerObjects.Keys)
        {
            if (!state.Players.ContainsKey(id))
            {
                toRemove.Add(id);
            }
        }

        foreach (var id in toRemove)
        {
            RemovePlayer(id);
        }
    }

    private void SpawnPlayer(int id)
    {
        Vector2 spawnPos = spawnManager != null
            ? spawnManager.GetSpawnPosition(id)
            : Vector2.zero;

        GameObject obj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        obj.name = $"Player_{id}";

        var tag = obj.GetComponent<PlayerTag>();
        if (tag != null)
        {
            tag.PlayerId = id;
        }

        if (state.Players.TryGetValue(id, out var playerData))
        {
            playerData.Position = spawnPos;
        }

        playerObjects[id] = obj;
        Debug.Log("SPAWN PLAYER: " + id);
    }

    private void UpdatePlayer(PlayerData data)
    {
        if (!playerObjects.TryGetValue(data.Id, out GameObject obj))
            return;

        if (data.Status == PlayerStatus.Spectator)
        {
            Destroy(obj);
            playerObjects.Remove(data.Id);
            return;
        }

        obj.transform.position = new Vector3(data.Position.x, data.Position.y, 0);
        Debug.Log("UPDATE PLAYER: " + data.Id + " STATUS: " + data.Status);
    }

    private void RemovePlayer(int id)
    {
        if (playerObjects.TryGetValue(id, out GameObject obj))
        {
            Destroy(obj);
            playerObjects.Remove(id);
        }
    }
}