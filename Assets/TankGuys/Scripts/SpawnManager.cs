using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public Vector2 GetSpawnPosition(int playerId)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return Vector2.zero;

        int index = playerId % spawnPoints.Length;

        return spawnPoints[index].position;
    }
}