using UnityEngine;
using UnityEngine.EventSystems;

public class RelicHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RelicDataSO relic;
    private RelicTooltipUI tooltipUI;

    public void Initialize(RelicDataSO relic, RelicTooltipUI tooltipUI)
    {
        this.relic = relic;
        this.tooltipUI = tooltipUI;
    }

    public void Clear()
    {
        if (tooltipUI != null)
            tooltipUI.Hide();

        relic = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (relic == null || tooltipUI == null)
            return;

        tooltipUI.Show(relic);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipUI == null)
            return;

        tooltipUI.Hide();
    }
}
