using UnityEngine;
using UnityEngine.EventSystems;

public class StatusEffectHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string statusEffectName;
    private string description;
    private StatusEffectTooltipUI tooltipUI;

    public void Initialize(string statusEffectName, string description, StatusEffectTooltipUI tooltipUI)
    {
        this.statusEffectName = statusEffectName;
        this.description = description;
        this.tooltipUI = tooltipUI;
    }

    public void Clear()
    {
        if (tooltipUI != null)
            tooltipUI.Hide();

        statusEffectName = null;
        description = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(statusEffectName) || tooltipUI == null)
            return;

        tooltipUI.Show(statusEffectName, description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipUI == null)
            return;

        tooltipUI.Hide();
    }
}
