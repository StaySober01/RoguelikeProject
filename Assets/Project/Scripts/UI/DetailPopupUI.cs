using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DetailPopupUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI emptyText;

    [Header("Card List")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private DetailPopupCardItemUI cardItemPrefab;

    [Header("Tooltip")]
    [SerializeField] private CardTooltipUI cardTooltipUI;

    private readonly List<DetailPopupCardItemUI> spawnedItems = new();

    private void Awake()
    {
        Hide();
    }

    public void ShowCardList(string title, IReadOnlyList<CardInstance> cards)
    {
        ClearItems();

        if (titleText != null)
            titleText.text = title;

        bool isEmpty = cards == null || cards.Count == 0;

        if (emptyText != null)
        {
            emptyText.gameObject.SetActive(isEmpty);
            if (isEmpty)
                emptyText.text = "Empty";
        }

        if (!isEmpty)
        {
            foreach (var card in cards)
            {
                var item = Instantiate(cardItemPrefab, contentRoot);
                item.Initialize(card, cardTooltipUI);
                spawnedItems.Add(item);
            }
        }

        root.SetActive(true);
    }

    public void ShowTextList(string title, string content)
    {
        ClearItems();

        if (titleText != null)
            titleText.text = title;

        if (emptyText != null)
        {
            emptyText.gameObject.SetActive(true);
            emptyText.text = content;
        }

        root.SetActive(true);
    }

    public void Hide()
    {
        ClearItems();

        if (cardTooltipUI != null)
            cardTooltipUI.Hide();

        root.SetActive(false);
    }

    private void ClearItems()
    {
        foreach (var item in spawnedItems)
        {
            if (item != null)
            {
                item.Clear();
                Destroy(item.gameObject);
            }
        }

        spawnedItems.Clear();

        if (emptyText != null)
            emptyText.gameObject.SetActive(false);
    }
}