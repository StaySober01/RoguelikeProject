using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CardInstance card;
    private CardTooltipUI tooltipUI;

    public void Initialize(CardInstance card, CardTooltipUI tooltipUI)
    {
        this.card = card;
        this.tooltipUI = tooltipUI;
    }

    public void Clear()
    {
        if (tooltipUI != null)
            tooltipUI.Hide();

        card = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (card == null || tooltipUI == null)
            return;

        tooltipUI.Show(card);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipUI == null)
            return;

        tooltipUI.Hide();
    }
}