using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    public void Spawn(int playerId, Transform shootPoint, Vector2 direction)
    {
        if (shootPoint == null) return;

        GameObject proj = Instantiate(
            projectilePrefab,
            shootPoint.position,
            shootPoint.rotation
        );

        var projectile = proj.GetComponent<Projectile>();
        projectile.Initialize(playerId, direction.normalized);
    }
}