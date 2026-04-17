using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StatusEffectTooltipUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private Canvas canvas;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Follow Mouse")]
    [SerializeField] private Vector2 offset = new Vector2(-5f, 5f);

    private bool isShowing = false;

    private void Awake()
    {
        Hide();
    }

    private void Update()
    {
        if (!isShowing)
            return;

        FollowMouse();
    }

    public void Show(string statusEffectName, string description)
    {
        if (string.IsNullOrEmpty(statusEffectName))
            return;

        if (nameText != null)
            nameText.text = statusEffectName;

        if (descriptionText != null)
            descriptionText.text = description;

        if (root != null)
            root.SetActive(true);

        isShowing = true;
        FollowMouse();
    }

    public void Hide()
    {
        isShowing = false;

        if (root != null)
            root.SetActive(false);
    }

    private void FollowMouse()
    {
        if (tooltipRect == null || canvas == null)
            return;

        RectTransform canvasRect = canvas.transform as RectTransform;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mousePos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        tooltipRect.anchoredPosition = localPoint + offset;
    }
}
