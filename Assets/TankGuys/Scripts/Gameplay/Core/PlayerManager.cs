using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public GameObject[] playerPrefabs;

    private Dictionary<int, GameObject> playerObjects = new();
    private Dictionary<int, Queue<Vector3>> positionBuffers = new();
    private Dictionary<int, Animator> playerAnimators = new();

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

        GameObject prefab = GetPrefabForPlayer(id);

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
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

        positionBuffers[id] = new Queue<Vector3>();

        var anim = obj.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            playerAnimators[id] = anim;
        }
    }

    private GameObject GetPrefabForPlayer(int id)
    {
        if (playerPrefabs == null || playerPrefabs.Length == 0)
            return null;

        int index = (id - 1) % playerPrefabs.Length;

        return playerPrefabs[index];
    }

    private void UpdatePlayer(PlayerData data)
    {
        if (!playerObjects.TryGetValue(data.Id, out GameObject obj))
            return;

        if (data.Status == PlayerStatus.Spectator)
        {
            Destroy(obj);
            playerObjects.Remove(data.Id);
            positionBuffers.Remove(data.Id);
            playerAnimators.Remove(data.Id);
            return;
        }

        Vector3 targetPos = new Vector3(data.Position.x, data.Position.y, 0);

        if (!positionBuffers.ContainsKey(data.Id))
        {
            positionBuffers[data.Id] = new Queue<Vector3>();
        }

        var buffer = positionBuffers[data.Id];
        buffer.Enqueue(targetPos);

        if (buffer.Count > 5)
        {
            buffer.Dequeue();
        }

        Vector3 currentPos = obj.transform.position;
        Vector3 newPos = currentPos;

        if (buffer.Count >= 2)
        {
            var positions = buffer.ToArray();
            Vector3 to = positions[1];
            float t = 10f * Time.deltaTime;
            newPos = Vector3.Lerp(currentPos, to, t);
        }

        bool isMoving = (targetPos - currentPos).sqrMagnitude > 0.0001f;

        obj.transform.position = newPos;

        if (playerAnimators.TryGetValue(data.Id, out var anim))
        {
            anim.SetBool("IsMoving", isMoving);
        }
    }

    private void RemovePlayer(int id)
    {
        if (playerObjects.TryGetValue(id, out GameObject obj))
        {
            Destroy(obj);
            playerObjects.Remove(id);
            positionBuffers.Remove(id);
            playerAnimators.Remove(id);
        }
    }
}