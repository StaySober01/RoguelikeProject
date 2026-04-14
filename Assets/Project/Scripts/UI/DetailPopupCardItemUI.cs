using TMPro;
using UnityEngine;

public class DetailPopupCardItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private CardInstance card;

    public void Initialize(CardInstance card, CardTooltipUI tooltipUI)
    {
        this.card = card;

        if (text != null && card != null)
        {
            text.text = $"{card.CardName} ({card.Cost})";
        }

        CardHoverHandler hoverHandler = GetComponent<CardHoverHandler>();
        if (hoverHandler == null)
        {
            hoverHandler = gameObject.AddComponent<CardHoverHandler>();
        }

        hoverHandler.Initialize(card, tooltipUI);
    }

    public void Clear()
    {
        card = null;

        CardHoverHandler hoverHandler = GetComponent<CardHoverHandler>();
        if (hoverHandler != null)
        {
            hoverHandler.Clear();
        }
    }
}