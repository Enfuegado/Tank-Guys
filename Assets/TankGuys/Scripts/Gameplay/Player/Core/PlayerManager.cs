using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public GameObject[] playerPrefabs;

    private Dictionary<int, GameObject> playerObjects = new();
    private Dictionary<int, Animator> playerAnimators = new();

    private GameState state;
    private SpawnManager spawnManager;

    private PlayerMovementInterpolator interpolator = new PlayerMovementInterpolator();
    private PlayerSpawner spawner;

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
        spawner = new PlayerSpawner(spawnManager, playerPrefabs);
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
        GameObject obj = spawner.Spawn(id, state);

        playerObjects[id] = obj;

        var anim = obj.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            playerAnimators[id] = anim;
        }

        bool isLocal = id == state.LocalPlayerId;

        if (isLocal)
        {
            if (!obj.TryGetComponent<PlayerLocalController>(out _))
                obj.AddComponent<PlayerLocalController>();
        }
        else
        {
            if (!obj.TryGetComponent<PlayerRemoteController>(out _))
                obj.AddComponent<PlayerRemoteController>();
        }
    }

    private void UpdatePlayer(PlayerData data)
    {
        if (!playerObjects.TryGetValue(data.Id, out GameObject obj))
            return;

        if (data.Status == PlayerStatus.Spectator)
        {
            RemovePlayer(data.Id);
            return;
        }

        Vector3 currentPos = obj.transform.position;
        Vector3 targetPos = new Vector3(data.Position.x, data.Position.y, 0);

        Vector3 newPos = interpolator.GetPosition(data.Id, currentPos, targetPos);

        obj.transform.position = newPos;

        bool isMoving = (targetPos - currentPos).sqrMagnitude > 0.0001f;

        if (playerAnimators.TryGetValue(data.Id, out var anim))
        {
            anim.SetBool("IsMoving", isMoving);
        }

        if (obj.TryGetComponent<PlayerLocalController>(out var local))
        {
            local.Tick(data);
        }
        else if (obj.TryGetComponent<PlayerRemoteController>(out var remote))
        {
            remote.Tick(data);
        }
    }

    private void RemovePlayer(int id)
    {
        if (playerObjects.TryGetValue(id, out GameObject obj))
        {
            Destroy(obj);
            playerObjects.Remove(id);
            playerAnimators.Remove(id);
            interpolator.Remove(id);
        }
    }

    public GameObject GetPlayerObject(int id)
    {
        if (playerObjects.TryGetValue(id, out var obj))
            return obj;

        return null;
    }
}