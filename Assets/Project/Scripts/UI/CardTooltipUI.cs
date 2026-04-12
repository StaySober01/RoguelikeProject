using TMPro;
using UnityEngine;

public class CardTooltipUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Awake()
    {
        Hide();
    }

    public void Show(CardInstance card)
    {
        if (card == null)
            return;

        root.SetActive(true);

        nameText.text = card.CardName;
        costText.text = $"({card.Cost})";
        categoryText.text = card.Category.ToString();
        descriptionText.text = card.Description;
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}