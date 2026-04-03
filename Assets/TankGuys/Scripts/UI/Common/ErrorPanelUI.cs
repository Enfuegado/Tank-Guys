using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ErrorPanelUI : MonoBehaviour
{
    public static ErrorPanelUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Button okButton;

    private MainMenuUI menu;

    void Awake()
    {
        Instance = this;

        panel.SetActive(false);

        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(OnOkClicked);
    }

    void Start()
    {
        menu = FindObjectOfType<MainMenuUI>();
    }

    public void Show(string message)
    {
        if (this == null || gameObject == null) return;

        panel.SetActive(true);
        errorText.text = message;
        Time.timeScale = 0f;
    }

    private void OnOkClicked()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;

        if (menu == null)
            menu = FindObjectOfType<MainMenuUI>();

        if (menu != null)
            menu.ResetUI();
    }
}