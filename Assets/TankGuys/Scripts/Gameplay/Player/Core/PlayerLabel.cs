using UnityEngine;

public class PlayerLabel : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    void Update()
    {
        if (target == null) return;

        transform.position = target.position + offset;
    }
}