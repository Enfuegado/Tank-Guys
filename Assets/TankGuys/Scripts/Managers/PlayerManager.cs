using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public GameObject playerPrefab;

    private Dictionary<int, GameObject> playerObjects = new();

    private PlayerRegistry registry;
    private NetworkState state;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Initialize(NetworkState networkState)
    {
        state = networkState;

        registry = new PlayerRegistry();

        registry.OnPlayerAdded += SpawnPlayer;
        registry.OnPlayerRemoved += RemovePlayer;

        state.OnPlayersUpdated += SyncPlayers;

        SyncPlayers();
    }

    void OnDestroy()
    {
        if (state != null)
        {
            state.OnPlayersUpdated -= SyncPlayers;
        }
    }

    private void SyncPlayers()
    {
        registry.Sync(state.Players);
    }

    private void SpawnPlayer(int id)
    {
        GameObject obj = Instantiate(playerPrefab);

        playerObjects[id] = obj;

        Debug.Log($"SPAWN PLAYER {id}");
    }

    private void RemovePlayer(int id)
    {
        if (playerObjects.TryGetValue(id, out GameObject obj))
        {
            Destroy(obj);
            playerObjects.Remove(id);

            Debug.Log($"REMOVE PLAYER {id}");
        }
    }
}