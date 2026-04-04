using UnityEngine;

public class ClientShootHandler : MonoBehaviour
{
    private GameClient client;
    private ProjectileSpawner spawner;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (NetworkBootstrap.Instance == null) return;

        client = NetworkBootstrap.Instance.ActiveClient;
        if (client == null) return;

        spawner = FindFirstObjectByType<ProjectileSpawner>();
        if (spawner == null) return;

        client.Logic.OnShoot += HandleShoot;
    }

    private void HandleShoot(int playerId, float dirX, float dirY)
    {
        GameObject playerObj = PlayerManager.Instance.GetPlayerObject(playerId);
        if (playerObj == null) return;

        var shootPoint = playerObj.transform.Find("Turret/Gun/ShootPoint");
        if (shootPoint == null) return;

        Vector2 direction = new Vector2(dirX, dirY);

        spawner.Spawn(playerId, shootPoint, direction);
    }

    private void OnDestroy()
    {
        if (client != null)
            client.Logic.OnShoot -= HandleShoot;
    }
}