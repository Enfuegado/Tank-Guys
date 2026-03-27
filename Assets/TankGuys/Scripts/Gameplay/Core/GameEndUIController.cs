using UnityEngine;
using TMPro;

public class GameEndUIController : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI winnerText;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void ShowWinner(int winnerId)
    {
        if (panel != null)
            panel.SetActive(true);

        if (winnerText != null)
            winnerText.text = "Player " + winnerId + " Wins!";
    }

    public void OnExitButton()
    {
        NetworkBootstrap.Instance.ResetNetwork();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}