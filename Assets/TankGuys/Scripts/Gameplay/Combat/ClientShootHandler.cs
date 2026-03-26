using UnityEngine;

public class ClientShootHandler : MonoBehaviour
{
    private GameClient client;
    private ProjectileSpawner spawner;

    void Start()
    {
        InvokeRepeating(nameof(TryInitialize), 0f, 0.5f);
    }

    void TryInitialize()
    {
        if (client != null) return;

        if (NetworkBootstrap.Instance == null) return;

        client = NetworkBootstrap.Instance.ActiveClient;
        if (client == null) return;

        spawner = FindObjectOfType<ProjectileSpawner>();
        if (spawner == null) return;

        client.Logic.OnShoot += HandleShoot;

        Debug.Log("ShootHandler INITIALIZED");

        CancelInvoke();
    }

    private void HandleShoot(int playerId, float dirX, float dirY)
    {
        Debug.Log("SPAWN PROJECTILE");

        var playerObj = FindPlayerObject(playerId);
        if (playerObj == null) return;

        var shootPoint = playerObj.transform.Find("Turret/Gun/ShootPoint");
        if (shootPoint == null) return;

        Vector2 direction = new Vector2(dirX, dirY);

        spawner.Spawn(playerId, shootPoint, direction);
    }

    private GameObject FindPlayerObject(int playerId)
    {
        var players = FindObjectsOfType<PlayerTag>();

        foreach (var p in players)
        {
            if (p.PlayerId == playerId)
                return p.gameObject;
        }

        return null;
    }

    private void OnDestroy()
    {
        if (client != null)
            client.Logic.OnShoot -= HandleShoot;
    }
}