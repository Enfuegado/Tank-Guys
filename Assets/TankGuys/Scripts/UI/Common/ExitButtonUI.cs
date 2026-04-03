using UnityEngine;

public class ExitButtonUI : MonoBehaviour
{
    public void OnExitButton()
    {
        NetworkBootstrap.Instance.ResetNetwork();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}