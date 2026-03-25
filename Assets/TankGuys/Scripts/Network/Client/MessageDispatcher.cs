using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageDispatcher
{
    private Dictionary<Type, IMessageHandler> handlers = new();

    private GameClient client;

    public MessageDispatcher(GameClient client)
    {
        this.client = client;
    }

    public void Register<T>(IMessageHandler handler) where T : NetMessage
    {
        handlers[typeof(T)] = handler;
    }

    public void Dispatch(NetMessage message)
    {
        var type = message.GetType();

        if (handlers.TryGetValue(type, out var handler))
        {
            handler.Handle(message, client);
        }
        else
        {
            Debug.LogWarning("No handler for message: " + type);
        }
    }
}