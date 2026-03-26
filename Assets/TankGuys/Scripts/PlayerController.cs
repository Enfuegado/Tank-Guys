using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform turret;

    private PlayerTag tag;

    void Start()
    {
        tag = GetComponent<PlayerTag>();
    }

    void Update()
    {
        if (NetworkBootstrap.Instance == null) return;

        var client = NetworkBootstrap.Instance.ActiveClient;
        if (client == null) return;

        if (tag == null) return;

        if (tag.PlayerId != client.State.LocalPlayerId)
            return;

        HandleRotation();
        Debug.Log($"Local: {client.State.LocalPlayerId} | Mine: {tag.PlayerId}");
    }

    private void HandleRotation()
    {
        if (turret == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 dir = mouseWorld - turret.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        turret.rotation = Quaternion.Euler(0, 0, angle);
    }
}