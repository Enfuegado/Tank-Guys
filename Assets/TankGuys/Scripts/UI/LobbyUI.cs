using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance;

    public Transform playerContainer;
    public GameObject playerRowPrefab;

    public TextMeshProUGUI statusText;
    public Button startGameButton;

    private Dictionary<int, PlayerRowUI> rows = new();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        startGameButton.onClick.RemoveAllListeners();
        startGameButton.onClick.AddListener(OnStartGameClicked);
    }

    void Update()
    {
        UpdatePlayersUI();
        DetectOutsideClick();
    }

    private void OnStartGameClicked()
    {
        GameManager.Instance?.TryStartGame();
    }

    void UpdatePlayersUI()
    {
        var state = NetworkBootstrap.Instance.State;
        if (state == null) return;

        var orderedIds = state.Players.Keys.OrderBy(id => id).ToList();

        foreach (var id in orderedIds)
        {
            if (!rows.ContainsKey(id))
            {
                var obj = Instantiate(playerRowPrefab, playerContainer);
                var row = obj.GetComponent<PlayerRowUI>();
                row.Setup(id);

                rows[id] = row;
            }
        }

        var toRemove = rows.Keys.Where(id => !orderedIds.Contains(id)).ToList();

        foreach (var id in toRemove)
        {
            Destroy(rows[id].gameObject);
            rows.Remove(id);
        }

        for (int i = 0; i < orderedIds.Count; i++)
        {
            var id = orderedIds[i];
            rows[id].transform.SetSiblingIndex(i);
        }

        startGameButton.interactable = state.Players.Count >= 2;

        statusText.text = state.Players.Count < 2
            ? "Esperando más jugadores..."
            : "Listo para iniciar";
    }

    void DetectOutsideClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                CloseAll();
            }
        }
    }

    public void CloseAll()
    {
        foreach (var row in rows.Values)
        {
            row.Close();
        }
    }
}