using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectIconUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI stackText;

    public void Initialize(
        Sprite iconSprite,
        int stack,
        string statusEffectName,
        string description,
        StatusEffectTooltipUI tooltipUI)
    {
        if (iconImage != null)
            iconImage.sprite = iconSprite;

        if (stackText != null)
            stackText.text = stack.ToString();

        StatusEffectHoverHandler hoverHandler = GetComponent<StatusEffectHoverHandler>();
        if (hoverHandler == null)
            hoverHandler = gameObject.AddComponent<StatusEffectHoverHandler>();

        hoverHandler.Initialize(statusEffectName, description, tooltipUI);
    }

    public void Clear()
    {
        StatusEffectHoverHandler hoverHandler = GetComponent<StatusEffectHoverHandler>();
        if (hoverHandler != null)
            hoverHandler.Clear();
    }
}
