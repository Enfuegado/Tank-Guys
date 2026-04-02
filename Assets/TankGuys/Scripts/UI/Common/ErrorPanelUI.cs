using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ErrorPanelUI : MonoBehaviour
{
    public static ErrorPanelUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Button okButton;

    void Awake()
    {
        Instance = this;

        panel.SetActive(false);

        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(Hide);
    }

    public void Show(string message)
    {
        if (this == null || gameObject == null) return;

        panel.SetActive(true);
        errorText.text = message;
        Time.timeScale = 0f;
    }

    private void Hide()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}