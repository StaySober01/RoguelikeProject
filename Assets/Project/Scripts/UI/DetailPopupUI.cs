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

    [Header("Shared Content")]
    [SerializeField] private Transform contentRoot;

    [Header("Card List")]
    [SerializeField] private DetailPopupCardItemUI cardItemPrefab;

    [Header("Relic List")]
    [SerializeField] private DetailPopupRelicItemUI relicItemPrefab;

    [Header("Tooltip")]
    [SerializeField] private CardTooltipUI cardTooltipUI;
    [SerializeField] private RelicTooltipUI relicTooltipUI;

    private readonly List<DetailPopupCardItemUI> spawnedCardItems = new();
    private readonly List<DetailPopupRelicItemUI> spawnedRelicItems = new();

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
                spawnedCardItems.Add(item);
            }
        }

        root.SetActive(true);
    }

    public void ShowRelicList(string title, IReadOnlyList<RelicDataSO> relics)
    {
        ClearItems();

        if (titleText != null)
            titleText.text = title;

        bool isEmpty = relics == null || relics.Count == 0;

        if (emptyText != null)
        {
            emptyText.gameObject.SetActive(isEmpty);
            if (isEmpty)
                emptyText.text = "Empty";
        }

        if (!isEmpty)
        {
            foreach (var relic in relics)
            {
                var item = Instantiate(relicItemPrefab, contentRoot);
                item.Initialize(relic, relicTooltipUI);
                spawnedRelicItems.Add(item);
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

        if (relicTooltipUI != null)
            relicTooltipUI.Hide();

        root.SetActive(false);
    }

    private void ClearItems()
    {
        foreach (var item in spawnedCardItems)
        {
            if (item != null)
            {
                item.Clear();
                Destroy(item.gameObject);
            }
        }

        spawnedCardItems.Clear();

        foreach (var item in spawnedRelicItems)
        {
            if (item != null)
            {
                item.Clear();
                Destroy(item.gameObject);
            }
        }

        spawnedRelicItems.Clear();

        if (emptyText != null)
            emptyText.gameObject.SetActive(false);
    }
}