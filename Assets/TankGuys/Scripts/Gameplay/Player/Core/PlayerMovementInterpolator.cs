using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementInterpolator
{
    private Dictionary<int, Queue<Vector3>> buffers = new();

    public Vector3 GetPosition(int id, Vector3 current, Vector3 target)
    {
        if (!buffers.ContainsKey(id))
            buffers[id] = new Queue<Vector3>();

        var buffer = buffers[id];

        buffer.Enqueue(target);

        if (buffer.Count > 5)
            buffer.Dequeue();

        if (buffer.Count < 2)
            return current;

        var positions = buffer.ToArray();
        Vector3 to = positions[1];

        float t = 10f * Time.deltaTime;

        return Vector3.Lerp(current, to, t);
    }

    public void Remove(int id)
    {
        if (buffers.ContainsKey(id))
            buffers.Remove(id);
    }
}