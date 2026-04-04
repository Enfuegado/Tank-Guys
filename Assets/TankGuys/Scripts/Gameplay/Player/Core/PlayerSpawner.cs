using UnityEngine;

public class PlayerSpawner
{
    private SpawnManager spawnManager;
    private GameObject[] prefabs;

    public PlayerSpawner(SpawnManager spawnManager, GameObject[] prefabs)
    {
        this.spawnManager = spawnManager;
        this.prefabs = prefabs;
    }

    public GameObject Spawn(int id, GameState state)
    {
        Vector2 spawnPos = spawnManager != null
            ? spawnManager.GetSpawnPosition(id)
            : Vector2.zero;

        GameObject prefab = GetPrefab(id);

        GameObject obj = Object.Instantiate(prefab, spawnPos, Quaternion.identity);
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

        return obj;
    }

    private GameObject GetPrefab(int id)
    {
        if (prefabs == null || prefabs.Length == 0)
            return null;

        int index = (id - 1) % prefabs.Length;

        return prefabs[index];
    }
}