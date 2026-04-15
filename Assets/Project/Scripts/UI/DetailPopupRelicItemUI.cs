using TMPro;
using UnityEngine;

public class DetailPopupRelicItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private RelicDataSO relic;

    public void Initialize(RelicDataSO relic, RelicTooltipUI tooltipUI)
    {
        this.relic = relic;

        if (text != null && relic != null)
        {
            text.text = relic.DisplayName;
        }

        RelicHoverHandler hoverHandler = GetComponent<RelicHoverHandler>();
        if (hoverHandler == null)
        {
            hoverHandler = gameObject.AddComponent<RelicHoverHandler>();
        }

        hoverHandler.Initialize(relic, tooltipUI);
    }

    public void Clear()
    {
        relic = null;

        RelicHoverHandler hoverHandler = GetComponent<RelicHoverHandler>();
        if (hoverHandler != null)
        {
            hoverHandler.Clear();
        }
    }
}
