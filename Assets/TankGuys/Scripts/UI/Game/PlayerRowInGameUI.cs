using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerRowInGameUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI pingText;

    public Image[] hearts;

    private int playerId;

    public void Setup(int id, int localId)
    {
        playerId = id;

        nameText.text = "Player " + id;

        if (id == localId)
        {
            nameText.fontSize += 6;
            nameText.color = Color.cyan;
        }
    }

    public void UpdateLives(int lives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].gameObject.SetActive(i < lives);
        }
    }

    public void UpdatePing(int ping)
    {
        pingText.text = ping + " ms";
    }
}