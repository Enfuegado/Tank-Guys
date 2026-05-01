using UnityEngine;
using UnityEngine.UI;

public class CreditsPanelUI : MonoBehaviour
{
    public Button okButton;

    void OnEnable()
    {
        okButton.onClick.AddListener(CloseCredits);
    }

    void OnDisable()
    {
        okButton.onClick.RemoveListener(CloseCredits);
    }

    private void CloseCredits() => gameObject.SetActive(false);
}