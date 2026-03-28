using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InGamePlayerListUI : MonoBehaviour
{
    public Transform container;
    public GameObject rowPrefab;

    private Dictionary<int, PlayerRowInGameUI> rows = new();

    void Update()
    {
        var state = NetworkBootstrap.Instance.State;
        if (state == null) return;

        var orderedIds = state.Players.Keys.OrderBy(id => id).ToList();

        foreach (var id in orderedIds)
        {
            if (!rows.ContainsKey(id))
            {
                var obj = Instantiate(rowPrefab, container);
                var row = obj.GetComponent<PlayerRowInGameUI>();

                row.Setup(id, state.LocalPlayerId);

                rows[id] = row;
            }

            var player = state.Players[id];

            rows[id].UpdateLives(player.Lives);

            int ping = -1;

            if (PingSystem.Instance != null)
                ping = PingSystem.Instance.GetPing(id);

            if (ping >= 0)
                rows[id].UpdatePing(ping);
            else
                rows[id].UpdatePing(0);
        }

        var toRemove = rows.Keys.Where(id => !orderedIds.Contains(id)).ToList();

        foreach (var id in toRemove)
        {
            Destroy(rows[id].gameObject);
            rows.Remove(id);
        }

        int index = 0;
        foreach (var id in orderedIds)
        {
            rows[id].transform.SetSiblingIndex(index);
            index++;
        }
    }
}