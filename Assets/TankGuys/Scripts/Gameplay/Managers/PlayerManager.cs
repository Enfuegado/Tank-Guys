using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public GameObject playerPrefab;

    private Dictionary<int, GameObject> playerObjects = new();

    private GameState state;

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
        GameObject obj = Instantiate(playerPrefab);
        obj.name = $"Player_{id}";
        playerObjects[id] = obj;
    }

    private void UpdatePlayer(PlayerData data)
    {
        if (playerObjects.TryGetValue(data.Id, out GameObject obj))
        {
            obj.transform.position = new Vector3(data.Position.x, 0, data.Position.y);
        }
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