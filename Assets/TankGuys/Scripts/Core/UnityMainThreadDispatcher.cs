using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher instance;

    private static readonly Queue<Action> executionQueue = new Queue<Action>();

    public static UnityMainThreadDispatcher Instance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<UnityMainThreadDispatcher>();

            if (instance == null)
            {
                GameObject obj = new GameObject("MainThreadDispatcher");
                instance = obj.AddComponent<UnityMainThreadDispatcher>();
                DontDestroyOnLoad(obj);
            }
        }

        return instance;
    }

    public void Enqueue(Action action)
    {
        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        while (true)
        {
            Action action = null;

            lock (executionQueue)
            {
                if (executionQueue.Count > 0)
                {
                    action = executionQueue.Dequeue();
                }
            }

            if (action == null) break;

            action.Invoke();
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}