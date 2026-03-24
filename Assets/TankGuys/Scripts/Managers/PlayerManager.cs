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
        Debug.Log("GameManager creado: " + GetInstanceID());
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

        if (registry != null)
        {
            registry.OnPlayerAdded -= SpawnPlayer;
            registry.OnPlayerRemoved -= RemovePlayer;
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

        if (id == state.MyPlayerId)
        {
            obj.name = $"Player_{id} (LOCAL)";
            SetupLocalPlayer(obj);
        }
        else
        {
            obj.name = $"Player_{id}";
            SetupRemotePlayer(obj);
        }

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

    private void SetupLocalPlayer(GameObject obj)
    {
        // Aquí irá:
        // - cámara
        // - input
        // - control directo
    }

    private void SetupRemotePlayer(GameObject obj)
    {
        // Aquí irá:
        // - interpolación
        // - smoothing
    }
}