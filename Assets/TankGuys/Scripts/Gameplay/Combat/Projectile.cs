using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int ownerId;
    private Vector2 direction;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;

    private float timer;
    private bool hasHit = false;

    public void Initialize(int ownerId, Vector2 direction)
    {
        this.ownerId = ownerId;
        this.direction = direction.normalized;
    }

    void Update()
    {
        if (hasHit) return; 

        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        var player = other.GetComponent<PlayerTag>();
        if (player == null) return;

        if (player.PlayerId == ownerId)
            return;

        hasHit = true;

        if (NetworkBootstrap.Instance.IsHost)
        {
            NetworkBootstrap.Instance.Send(new DamageMessage
            {
                targetId = player.PlayerId
            });
        }

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}