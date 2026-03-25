using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageRouter
{
    private Dictionary<MessageType, Action<string>> handlers = new();

    public void Register<T>(MessageType type, Action<T> handler) where T : NetMessage
    {
        handlers[type] = (json) =>
        {
            T msg = JsonUtility.FromJson<T>(json);
            handler(msg);
        };
    }

    public void Handle(string json)
    {
        MessageWrapper wrapper = JsonUtility.FromJson<MessageWrapper>(json);

        if (handlers.TryGetValue(wrapper.type, out var handler))
        {
            handler(wrapper.json);
        }
        else
        {
            Debug.LogWarning("Tipo desconocido: " + wrapper.type);
        }
    }
}