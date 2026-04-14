using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailPopupUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        Hide();
    }

    public void Show(string title, string content)
    {
        if (titleText != null)
            titleText.text = title;

        if (contentText != null)
            contentText.text = content;

        if (root != null)
            root.SetActive(true);
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }
}