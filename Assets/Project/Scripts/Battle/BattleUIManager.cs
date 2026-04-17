using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;

public class BattleUIManager : MonoBehaviour
{
    [Header("Hand UI")]
    [SerializeField] private Button[] handButtons;
    [SerializeField] private TextMeshProUGUI[] handButtonTexts;
    [SerializeField] private CardTooltipUI cardTooltipUI;

    [Header("Turn UI")]
    [SerializeField] private Button endTurnButton;

    [Header("Reward UI")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private Button[] rewardButtons;
    [SerializeField] private TextMeshProUGUI[] rewardButtonTexts;
    [SerializeField] private Button skipRewardButton;

    [Header("Start Passive UI")]
    [SerializeField] private GameObject chooseStartPassivePanel;
    [SerializeField] private Button[] startPassiveButtons;
    [SerializeField] private TextMeshProUGUI[] startPassiveButtonTexts;

    [Header("Debug UI")]
    [SerializeField] private TextMeshProUGUI playerHpText;
    [SerializeField] private TextMeshProUGUI enemyHpText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI battleText;

    [Header("Battle Log UI")]
    [SerializeField] private BattleLogUI battleLogUI;

    [Header("Status Effect UI")]
    [SerializeField] private Transform playerStatusEffectRoot;
    [SerializeField] private Transform enemyStatusEffectRoot;
    [SerializeField] private StatusEffectIconUI statusEffectIconPrefab;
    [SerializeField] private StatusEffectTooltipUI statusEffectTooltipUI;
    [SerializeField] private List<StatusEffectDisplayEntry> statusEffectDisplayEntries = new();

    [Header("Detail Popup UI")]
    [SerializeField] private DetailPopupUI detailPopupUI;
    [SerializeField] private Button deckButton;
    [SerializeField] private Button drawPileButton;
    [SerializeField] private Button discardButton;
    [SerializeField] private Button relicButton;

    private BattleManager battleManager;
    private readonly List<StatusEffectIconUI> spawnedPlayerStatusEffectIcons = new();
    private readonly List<StatusEffectIconUI> spawnedEnemyStatusEffectIcons = new();

    private void Start()
    {
        battleManager = BattleManager.Instance;
        if (deckButton != null)
            deckButton.onClick.AddListener(OnClickDeck);

        if (drawPileButton != null)
            drawPileButton.onClick.AddListener(OnClickDrawPile);

        if (discardButton != null)
            discardButton.onClick.AddListener(OnClickDiscard);

        if (relicButton != null)
            relicButton.onClick.AddListener(OnClickRelic);
    }

    public void RefreshUI(
        IReadOnlyList<CardInstance> hand,
        BattleState state,
        int currentEnergy,
        int maxEnergy,
        int drawPileCount,
        int handCount,
        int discardPileCount,
        int battleWinCount,
        Unit playerUnit,
        Unit enemyUnit,
        StatusEffectController statusEffectController,
        Action<CardInstance> onUseCard)
    {
        UpdateHandUI(hand, state, currentEnergy, onUseCard);
        SetEndTurnInteractable(state == BattleState.PlayerTurn);
        UpdateStatusEffectUI(playerUnit, enemyUnit, statusEffectController);
        UpdateDebugUI(
            state,
            currentEnergy,
            maxEnergy,
            drawPileCount,
            handCount,
            discardPileCount,
            battleWinCount,
            playerUnit,
            enemyUnit,
            statusEffectController);
    }

    public void UpdateHandUI(
    IReadOnlyList<CardInstance> hand,
    BattleState state,
    int currentEnergy,
    Action<CardInstance> onUseCard)
    {
        if (handButtons == null || handButtonTexts == null)
            return;

        for (int i = 0; i < handButtons.Length; i++)
        {
            if (i < hand.Count)
            {
                CardInstance card = hand[i];

                handButtons[i].gameObject.SetActive(true);
                handButtonTexts[i].text = $"{card.CardName} ({card.Cost})";

                handButtons[i].onClick.RemoveAllListeners();

                if (onUseCard != null)
                    handButtons[i].onClick.AddListener(() => onUseCard(card));

                handButtons[i].interactable =
                    state == BattleState.PlayerTurn &&
                    currentEnergy >= card.Cost;

                CardHoverHandler hoverHandler = handButtons[i].GetComponent<CardHoverHandler>();
                if (hoverHandler == null)
                {
                    hoverHandler = handButtons[i].gameObject.AddComponent<CardHoverHandler>();
                }

                hoverHandler.Initialize(card, cardTooltipUI);
            }
            else
            {
                handButtons[i].onClick.RemoveAllListeners();

                CardHoverHandler hoverHandler = handButtons[i].GetComponent<CardHoverHandler>();
                if (hoverHandler != null)
                {
                    hoverHandler.Clear();
                }

                handButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateDebugUI(
        BattleState state,
        int currentEnergy,
        int maxEnergy,
        int drawPileCount,
        int handCount,
        int discardPileCount,
        int battleWinCount,
        Unit playerUnit,
        Unit enemyUnit,
        StatusEffectController statusEffectController)
    {
        if (playerHpText != null && playerUnit != null)
        {
            playerHpText.text =
                $"Player HP: {playerUnit.currentHp}/{playerUnit.maxHp}  " +
                $"Block: {playerUnit.currentBlock}";
        }

        if (enemyHpText != null && enemyUnit != null)
        {
            enemyHpText.text =
                $"Enemy HP: {enemyUnit.currentHp}/{enemyUnit.maxHp}  " +
                $"Block: {enemyUnit.currentBlock}";
        }

        if (energyText != null)
            energyText.text = $"Energy: {currentEnergy}/{maxEnergy}";

        if (battleText != null)
            battleText.text = $"Battle {battleWinCount + 1}";
    }

    public void UpdateStatusEffectUI(
        Unit playerUnit,
        Unit enemyUnit,
        StatusEffectController statusEffectController)
    {
        UpdateUnitStatusEffectIcons(
            playerStatusEffectRoot,
            spawnedPlayerStatusEffectIcons,
            playerUnit,
            statusEffectController);

        UpdateUnitStatusEffectIcons(
            enemyStatusEffectRoot,
            spawnedEnemyStatusEffectIcons,
            enemyUnit,
            statusEffectController);
    }

    public void ShowCardRewardUI(
        IReadOnlyList<CardInstance> rewardCardChoices,
        Action<CardInstance> onSelectReward,
        Action onSkipReward)
    {
        if (rewardPanel != null)
            rewardPanel.SetActive(true);

        if (rewardButtons == null || rewardButtonTexts == null)
            return;

        for (int i = 0; i < rewardButtons.Length; i++)
        {
            if (i < rewardCardChoices.Count)
            {
                CardInstance card = rewardCardChoices[i];

                rewardButtons[i].gameObject.SetActive(true);
                rewardButtonTexts[i].text = $"{card.CardName} ({card.Cost})";

                rewardButtons[i].onClick.RemoveAllListeners();

                if (onSelectReward != null)
                    rewardButtons[i].onClick.AddListener(() => onSelectReward(card));
            }
            else
            {
                rewardButtons[i].onClick.RemoveAllListeners();
                rewardButtons[i].gameObject.SetActive(false);
            }
        }

        BindSkipReward(onSkipReward);
    }

    public void ShowRelicRewardUI(
        IReadOnlyList<RelicDataSO> rewardRelicChoices,
        Action<RelicDataSO> onSelectReward,
        Action onSkipReward)
    {
        if (rewardPanel != null)
            rewardPanel.SetActive(true);

        if (rewardButtons == null || rewardButtonTexts == null)
            return;

        for (int i = 0; i < rewardButtons.Length; i++)
        {
            if (i < rewardRelicChoices.Count)
            {
                RelicDataSO relic = rewardRelicChoices[i];

                rewardButtons[i].gameObject.SetActive(true);
                rewardButtonTexts[i].text = relic.DisplayName;

                rewardButtons[i].onClick.RemoveAllListeners();

                if (onSelectReward != null)
                    rewardButtons[i].onClick.AddListener(() => onSelectReward(relic));
            }
            else
            {
                rewardButtons[i].onClick.RemoveAllListeners();
                rewardButtons[i].gameObject.SetActive(false);
            }
        }

        BindSkipReward(onSkipReward);
    }

    public void HideRewardUI()
    {
        if (rewardPanel != null)
            rewardPanel.SetActive(false);

        if (rewardButtons != null)
        {
            for (int i = 0; i < rewardButtons.Length; i++)
                rewardButtons[i].onClick.RemoveAllListeners();
        }

        if (skipRewardButton != null)
        {
            skipRewardButton.onClick.RemoveAllListeners();
            skipRewardButton.gameObject.SetActive(false);
        }
    }

    public void ShowStartPassiveSelection(
        IReadOnlyList<StartPassiveType> choices,
        Action<StartPassiveType> onSelectPassive)
    {
        if (chooseStartPassivePanel != null)
            chooseStartPassivePanel.SetActive(true);

        if (startPassiveButtons == null || startPassiveButtonTexts == null)
            return;

        for (int i = 0; i < startPassiveButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                StartPassiveType passive = choices[i];
                startPassiveButtons[i].gameObject.SetActive(true);
                startPassiveButtonTexts[i].text = passive.ToString();

                startPassiveButtons[i].onClick.RemoveAllListeners();

                if (onSelectPassive != null)
                    startPassiveButtons[i].onClick.AddListener(() => onSelectPassive(passive));
            }
            else
            {
                startPassiveButtons[i].onClick.RemoveAllListeners();
                startPassiveButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideStartPassiveSelection()
    {
        if (chooseStartPassivePanel != null)
            chooseStartPassivePanel.SetActive(false);
    }

    private void SetEndTurnInteractable(bool interactable)
    {
        if (endTurnButton != null)
            endTurnButton.interactable = interactable;
    }

    private void BindSkipReward(Action onSkipReward)
    {
        if (skipRewardButton == null)
            return;

        skipRewardButton.gameObject.SetActive(true);
        skipRewardButton.onClick.RemoveAllListeners();

        if (onSkipReward != null)
            skipRewardButton.onClick.AddListener(() => onSkipReward());
    }

    public void AddBattleLog(string message)
    {
        if (battleLogUI == null)
            return;

        battleLogUI.AddLog(message);
    }

    public void ClearBattleLogs()
    {
        if (battleLogUI == null)
            return;

        battleLogUI.ClearLogs();
    }

    public void ShowCardListPopup(string title, IReadOnlyList<CardInstance> cards)
    {
        if (detailPopupUI == null)
            return;

        detailPopupUI.ShowCardList(title, cards);
    }

    public void ShowRelicListPopup(string title, IReadOnlyList<RelicDataSO> relics)
    {
        if (detailPopupUI == null)
            return;

        detailPopupUI.ShowRelicList(title, relics);
    }

    private void OnClickDeck()
    {
        var deck = battleManager.GetCurrentDeck();
        ShowCardListPopup("Current Deck", deck);
    }

    private void OnClickDrawPile()
    {
        var drawPile = battleManager.GetDrawPile();
        ShowCardListPopup("Draw Pile", drawPile);
    }

    private void OnClickDiscard()
    {
        var discard = battleManager.GetDiscardPile();
        ShowCardListPopup("Discard Pile", discard);
    }

    private void OnClickRelic()
    {
        var relic = battleManager.GetCurrentRelic();
        ShowRelicListPopup("Active Relics", relic);
    }

    private void UpdateUnitStatusEffectIcons(
        Transform root,
        List<StatusEffectIconUI> spawnedIcons,
        Unit unit,
        StatusEffectController statusEffectController)
    {
        ClearStatusEffectIcons(spawnedIcons);

        if (root == null || statusEffectIconPrefab == null || unit == null || statusEffectController == null)
            return;

        foreach (StatusEffectType statusEffectType in GetDisplayStatusEffectTypes())
        {
            int stack = statusEffectController.GetStack(unit, statusEffectType);
            if (stack <= 0)
                continue;

            StatusEffectDisplayEntry displayEntry = GetStatusEffectDisplayEntry(statusEffectType);
            if (displayEntry == null)
                continue;

            StatusEffectIconUI iconUI = Instantiate(statusEffectIconPrefab, root);
            iconUI.Initialize(
                displayEntry.iconSprite,
                stack,
                displayEntry.displayName,
                displayEntry.description,
                statusEffectTooltipUI);
            spawnedIcons.Add(iconUI);
        }
    }

    private void ClearStatusEffectIcons(List<StatusEffectIconUI> spawnedIcons)
    {
        for (int i = 0; i < spawnedIcons.Count; i++)
        {
            if (spawnedIcons[i] == null)
                continue;

            spawnedIcons[i].Clear();
            Destroy(spawnedIcons[i].gameObject);
        }

        spawnedIcons.Clear();
    }

    private IEnumerable<StatusEffectType> GetDisplayStatusEffectTypes()
    {
        yield return StatusEffectType.Poison;
        yield return StatusEffectType.Burn;
        yield return StatusEffectType.Vulnerable;
    }

    private StatusEffectDisplayEntry GetStatusEffectDisplayEntry(StatusEffectType type)
    {
        for (int i = 0; i < statusEffectDisplayEntries.Count; i++)
        {
            if (statusEffectDisplayEntries[i] == null)
                continue;

            if (statusEffectDisplayEntries[i].type == type)
                return statusEffectDisplayEntries[i];
        }

        return null;
    }
}
