using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    #region Singleton

    public static BattleManager Instance { get; private set; }

    #endregion

    #region Fields

    public BattleState state;

    [Header("Units")]
    public Unit playerUnit;
    public Unit enemyUnit;

    [Header("Turn / Energy")]
    public int maxEnergy = 3;
    [SerializeField] private int currentEnergy;

    [Header("Battle Values")]
    public int enemyAttackDelay = 1;
    public float actionDelay = 0.5f;

    [Header("Enemy Scaling")]
    public int baseEnemyHp;
    public int baseEnemyAttack;

    [Header("Card System")]
    public int drawCountPerTurn = 5;
    public int maxHandSize = 5;
    public List<CardInstance> drawPile = new();
    public List<CardInstance> hand = new();
    public List<CardInstance> discardPile = new();
    public List<CardInstance> deck = new();
    private List<CardInstance> rewardCardChoices = new();

    [Header("Relics")]
    public List<RelicType> activeRelics = new();
    private List<RelicType> rewardRelicChoices = new();

    private RelicEffectCache relicEffectCache = new();

    [Header("UI - Hand")]
    public Button[] handButtons;
    public TextMeshProUGUI[] handButtonTexts;

    [Header("UI - Turn")]
    public Button endTurnButton;

    [Header("UI - Reward")]
    public GameObject rewardPanel;
    public Button[] rewardButtons;
    public TextMeshProUGUI[] rewardButtonTexts;
    public Button skipRewardButton;

    [Header("UI - Start Passive")]
    public GameObject chooseStartPassivePanel;
    public Button[] startPassiveButtons;
    public TextMeshProUGUI[] startPassiveButtonTexts;

    [Header("Debug UI")]
    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI enemyHpText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI stateText;

    [Header("Controllers")]
    public StatusEffectController statusEffectController;

    private bool doubleExplosionDamageThisTurn = false;
    private CardEffectResolver cardEffectResolver = new();
    private int battleWinCount = 0;
    private RewardType currentRewardType;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate BattleManager detected. Destroying this instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        baseEnemyHp = enemyUnit.maxHp;
        baseEnemyAttack = enemyUnit.attackPower;
        InitializePermanentDeck();
        ShowStartPassiveSelection();
    }

    #endregion

    #region Start Passive Selection

    private void ShowStartPassiveSelection()
    {
        if (chooseStartPassivePanel != null)
            chooseStartPassivePanel.SetActive(true);

        if (startPassiveButtons == null || startPassiveButtonTexts == null)
            return;

        StartPassiveType[] choices =
        {
            StartPassiveType.PoisonCore,
            StartPassiveType.BurnCore
        };

        for (int i = 0; i < startPassiveButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                StartPassiveType passive = choices[i];
                startPassiveButtons[i].gameObject.SetActive(true);
                startPassiveButtonTexts[i].text = passive.ToString();

                startPassiveButtons[i].onClick.RemoveAllListeners();
                startPassiveButtons[i].onClick.AddListener(() => SelectStartPassive(passive));
            }
            else
            {
                startPassiveButtons[i].onClick.RemoveAllListeners();
                startPassiveButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void SelectStartPassive(StartPassiveType passive)
    {
        playerUnit.selectedStartPassive = passive;
        Debug.Log($"Selected Start Passive: {passive}");

        if (chooseStartPassivePanel != null)
            chooseStartPassivePanel.SetActive(false);

        StartBattle();
    }

    public bool HasStartPassive(StartPassiveType startPassiveType)
    {
        return playerUnit.selectedStartPassive == startPassiveType;
    }

    #endregion

    #region Battle Flow

    public void StartBattle()
    {
        InitializeBattleDeck();
        ScaleEnemyForBattle();
        Debug.Log("Battle Start");
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        playerUnit.ResetBlock();
        currentEnergy = maxEnergy;
        doubleExplosionDamageThisTurn = false;
        statusEffectController.burnExplosionDamageMultiplier = 1;

        SetState(BattleState.PlayerTurn);

        DiscardHand();
        DrawCards(drawCountPerTurn);

        Debug.Log($"Player Turn Start - Energy: {currentEnergy}/{maxEnergy}");
        RefreshUI();
    }

    public void StartEnemyTurn()
    {
        SetState(BattleState.EnemyTurn);
        Debug.Log("Enemy Turn Start");
        RefreshUI();

        StartCoroutine(EnemyTurnRoutine());
    }

    public void OnClickEndTurn()
    {
        if (state != BattleState.PlayerTurn)
            return;

        Debug.Log("Player ends turn");
        RefreshUI();
        StartEnemyTurn();
    }

    private bool CheckBattleEnd()
    {
        if (enemyUnit.IsDead())
        {
            SetState(BattleState.Win);
            battleWinCount++;
            Debug.Log("Player Wins!");
            ShowBattleReward();
            RefreshUI();
            return true;
        }

        if (playerUnit.IsDead())
        {
            SetState(BattleState.Lose);
            Debug.Log("Player Loses...");
            RefreshUI();
            return true;
        }

        return false;
    }

    private void ScaleEnemyForBattle()
    {
        enemyUnit.maxHp = baseEnemyHp + (battleWinCount * 3);
        enemyUnit.currentHp = enemyUnit.maxHp;
        enemyUnit.attackPower = baseEnemyAttack + battleWinCount;
    }

    private void SetState(BattleState newState)
    {
        state = newState;
        UpdateDebugUI();
    }

    #endregion

    #region Enemy Turn

    private IEnumerator EnemyTurnRoutine()
    {
        SetState(BattleState.Busy);
        RefreshUI();

        yield return new WaitForSeconds(enemyAttackDelay);

        Debug.Log($"{enemyUnit.unitName} attacks Player");
        playerUnit.TakeDamage(enemyUnit.attackPower);
        RefreshUI();

        if (CheckBattleEnd())
            yield break;

        yield return new WaitForSeconds(actionDelay);

        statusEffectController.ProcessTurnEnd(enemyUnit);
        RefreshUI();

        if (CheckBattleEnd())
            yield break;

        yield return new WaitForSeconds(actionDelay);

        Debug.Log("Enemy Turn End");
        StartPlayerTurn();
    }

    #endregion

    #region Reward Flow

    private void GenerateCardRewards()
    {
        rewardCardChoices.Clear();

        List<CardInstance> rewardPool = CardFactory.CreateRewardCardPool();
        ShuffleCards(rewardPool);

        int rewardCount = Mathf.Min(3, rewardPool.Count);

        for (int i = 0; i < rewardCount; i++)
        {
            rewardCardChoices.Add(rewardPool[i]);
        }
    }

    public void SelectRewardCard(CardInstance card)
    {
        deck.Add(card);
        Debug.Log($"{card.CardName} added to permanent deck.");

        HideRewardUI();
        StartNextBattle();
    }

    private void GenerateRelicRewards()
    {
        rewardRelicChoices.Clear();

        List<RelicType> relicPool = new List<RelicType>
        {
            RelicType.VenomSac,
            RelicType.SmolderingAsh,
            RelicType.VolatileMixture
        };

        relicPool.RemoveAll(relic => activeRelics.Contains(relic));

        ShuffleRelics(relicPool);

        int rewardCount = Mathf.Min(3, relicPool.Count);

        for (int i = 0; i < rewardCount; i++)
        {
            rewardRelicChoices.Add(relicPool[i]);
        }
    }

    public void SelectRelicReward(RelicType relicType)
    {
        AddRelic(relicType);
        HideRewardUI();
        StartNextBattle();
    }

    private void ShowBattleReward()
    {
        if (ShouldShowRelicReward())
        {
            currentRewardType = RewardType.Relic;
            GenerateRelicRewards();
            ShowRelicRewardUI();
        }
        else
        {
            currentRewardType = RewardType.Card;
            GenerateCardRewards();
            ShowCardRewardUI();
        }
    }

    public void SkipReward()
    {
        Debug.Log("Player skipped card reward.");

        HideRewardUI();
        StartNextBattle();
    }

    private void StartNextBattle()
    {
        ResetUnitsForNextBattle();
        StartBattle();
    }

    private void ResetUnitsForNextBattle()
    {
        playerUnit.currentBlock = 0;
        enemyUnit.currentBlock = 0;

        playerUnit.ClearStatusData();
        enemyUnit.ClearStatusData();

        enemyUnit.currentHp = enemyUnit.maxHp;
    }

    private bool ShouldShowRelicReward()
    {
        return battleWinCount % 3 == 0;
    }

    #endregion

    #region Card System

    private void InitializePermanentDeck()
    {
        deck = CardFactory.CreateStarterDeck();
    }

    private void InitializeBattleDeck()
    {
        drawPile = new List<CardInstance>(deck);
        hand.Clear();
        discardPile.Clear();
        ShuffleCards(drawPile);
    }

    private void ShuffleCards(List<CardInstance> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            CardInstance temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    private void ReshuffleDiscardIntoDrawPile()
    {
        if (discardPile.Count == 0)
        {
            Debug.Log("No cards in discard pile to reshuffle.");
            return;
        }

        Debug.Log("Reshuffling discard pile into draw pile.");

        drawPile.AddRange(discardPile);
        discardPile.Clear();
        ShuffleCards(drawPile);
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (hand.Count >= maxHandSize)
            {
                Debug.Log("Hand is full.");
                break;
            }

            if (drawPile.Count == 0)
                ReshuffleDiscardIntoDrawPile();

            if (drawPile.Count == 0)
            {
                Debug.Log("No cards left to draw.");
                break;
            }

            CardInstance drawnCard = drawPile[0];
            drawPile.RemoveAt(0);
            hand.Add(drawnCard);

            Debug.Log($"Drew card: {drawnCard.CardName}");
        }

        UpdateHandUI();
    }

    private void DiscardHand()
    {
        if (hand.Count == 0)
        {
            UpdateHandUI();
            return;
        }

        Debug.Log($"Discarding {hand.Count} card(s) from hand.");

        discardPile.AddRange(hand);
        hand.Clear();

        UpdateHandUI();
    }

    public void UseCard(CardInstance card)
    {
        if (state != BattleState.PlayerTurn)
            return;

        if (!hand.Contains(card))
            return;

        if (currentEnergy < card.Cost)
        {
            Debug.Log($"Not enough energy to use {card.CardName}");
            return;
        }

        StartCoroutine(PlayerUseCardRoutine(card));
    }

    private IEnumerator PlayerUseCardRoutine(CardInstance card)
    {
        SetState(BattleState.Busy);
        RefreshUI();

        currentEnergy -= card.Cost;
        Debug.Log($"Player uses {card.CardName} (Cost: {card.Cost}, Energy: {currentEnergy}/{maxEnergy})");

        hand.Remove(card);
        discardPile.Add(card);

        UpdateHandUI();
        UpdateDebugUI();

        cardEffectResolver.Resolve(this, card);

        yield return new WaitForSeconds(actionDelay);

        if (CheckBattleEnd())
            yield break;

        SetState(BattleState.PlayerTurn);
        Debug.Log($"Player action complete. Remaining Energy: {currentEnergy}/{maxEnergy}");
        RefreshUI();
    }

    public void AddRandomCardWithTagFromDrawPileToHand(CardTag tag)
    {
        List<CardInstance> candidates = drawPile.FindAll(card => card.Tags.Contains(tag));

        if (candidates.Count == 0)
        {
            Debug.Log($"No {tag} cards found in draw pile.");
            return;
        }

        if (hand.Count >= maxHandSize)
        {
            Debug.Log("Hand is full. Cannot add searched card.");
            return;
        }

        CardInstance selectedCard = candidates[Random.Range(0, candidates.Count)];

        drawPile.Remove(selectedCard);
        hand.Add(selectedCard);

        Debug.Log($"Added {selectedCard.CardName} from draw pile to hand.");
        UpdateHandUI();
        UpdateDebugUI();
    }

    public void GainEnergy(int amount)
    {
        currentEnergy += amount;
        UpdateDebugUI();
        UpdateHandUI();
    }

    public void EnableDoubleExplosionDamageThisTurn()
    {
        doubleExplosionDamageThisTurn = true;
        statusEffectController.burnExplosionDamageMultiplier = 2;
    }

    #endregion

    #region Relic System
    public void RebuildRelicEffectCache()
    {
        relicEffectCache = new RelicEffectCache();

        foreach (RelicType relic in activeRelics)
        {
            switch (relic)
            {
                case RelicType.VenomSac:
                    relicEffectCache.bonusPoisonOnApply += 1;
                    break;

                case RelicType.SmolderingAsh:
                    relicEffectCache.drawOnBurnExplosion += 1;
                    break;

                case RelicType.VolatileMixture:
                    relicEffectCache.bonusDamageToPoisonAndBurnTarget += 2;
                    break;
            }
        }
    }

    public void AddRelic(RelicType relicType)
    {
        activeRelics.Add(relicType);
        RebuildRelicEffectCache();
        Debug.Log($"Relic acquired: {relicType}");
    }

    private void ShuffleRelics(List<RelicType> relics)
    {
        for (int i = 0; i < relics.Count; i++)
        {
            RelicType temp = relics[i];
            int randomIndex = Random.Range(i, relics.Count);
            relics[i] = relics[randomIndex];
            relics[randomIndex] = temp;
        }
    }

    public int GetBonusPoisonOnApply()
    {
        int amount = relicEffectCache.bonusPoisonOnApply;

        if (HasStartPassive(StartPassiveType.PoisonCore))
            amount += 1;

        return amount;
    }

    public int GetDrawOnBurnExplosion()
    {
        return relicEffectCache.drawOnBurnExplosion;
    }

    public int GetBonusDamageToPoisonAndBurnTarget(Unit target)
    {
        bool hasPoison = statusEffectController.HasStatus(target, StatusEffectType.Poison);
        bool hasBurn = statusEffectController.HasStatus(target, StatusEffectType.Burn);

        if (hasPoison && hasBurn)
            return relicEffectCache.bonusDamageToPoisonAndBurnTarget;

        return 0;
    }

    #endregion

    #region UI

    private void RefreshUI()
    {
        UpdateHandUI();

        if (endTurnButton != null)
            endTurnButton.interactable = (state == BattleState.PlayerTurn);

        UpdateDebugUI();
    }

    private void ShowCardRewardUI()
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
                rewardButtons[i].onClick.AddListener(() => SelectRewardCard(card));
            }
            else
            {
                rewardButtons[i].onClick.RemoveAllListeners();
                rewardButtons[i].gameObject.SetActive(false);
            }
        }

        if (skipRewardButton != null)
        {
            skipRewardButton.gameObject.SetActive(true);
            skipRewardButton.onClick.RemoveAllListeners();
            skipRewardButton.onClick.AddListener(SkipReward);
        }
    }

    private void ShowRelicRewardUI()
    {
        if (rewardPanel != null)
            rewardPanel.SetActive(true);

        if (rewardButtons == null || rewardButtonTexts == null)
            return;

        for (int i = 0; i < rewardButtons.Length; i++)
        {
            if (i < rewardRelicChoices.Count)
            {
                RelicType relic = rewardRelicChoices[i];

                rewardButtons[i].gameObject.SetActive(true);
                rewardButtonTexts[i].text = GetRelicDisplayName(relic);

                rewardButtons[i].onClick.RemoveAllListeners();
                rewardButtons[i].onClick.AddListener(() => SelectRelicReward(relic));
            }
            else
            {
                rewardButtons[i].onClick.RemoveAllListeners();
                rewardButtons[i].gameObject.SetActive(false);
            }
        }

        if (skipRewardButton != null)
        {
            skipRewardButton.gameObject.SetActive(true);
            skipRewardButton.onClick.RemoveAllListeners();
            skipRewardButton.onClick.AddListener(SkipReward);
        }
    }

    private void HideRewardUI()
    {
        rewardCardChoices.Clear();
        rewardRelicChoices.Clear();

        if (rewardPanel != null)
            rewardPanel.SetActive(false);

        if (rewardButtons != null)
        {
            for (int i = 0; i < rewardButtons.Length; i++)
            {
                rewardButtons[i].onClick.RemoveAllListeners();
            }
        }

        if (skipRewardButton != null)
        {
            skipRewardButton.onClick.RemoveAllListeners();
            skipRewardButton.gameObject.SetActive(false);
        }
    }

    private void UpdateHandUI()
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
                handButtons[i].onClick.AddListener(() => UseCard(card));

                handButtons[i].interactable =
                    state == BattleState.PlayerTurn &&
                    currentEnergy >= card.Cost;
            }
            else
            {
                handButtons[i].onClick.RemoveAllListeners();
                handButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateDebugUI()
    {
        int playerPoison = 0;
        int playerBurn = 0;
        int enemyPoison = 0;
        int enemyBurn = 0;

        if (statusEffectController != null && playerUnit != null && enemyUnit != null)
        {
            playerPoison = statusEffectController.GetStack(playerUnit, StatusEffectType.Poison);
            playerBurn = statusEffectController.GetStack(playerUnit, StatusEffectType.Burn);
            enemyPoison = statusEffectController.GetStack(enemyUnit, StatusEffectType.Poison);
            enemyBurn = statusEffectController.GetStack(enemyUnit, StatusEffectType.Burn);
        }

        if (playerHpText != null && playerUnit != null)
        {
            playerHpText.text =
                $"Player HP: {playerUnit.currentHp}/{playerUnit.maxHp}  " +
                $"Block: {playerUnit.currentBlock}  " +
                $"Poison: {playerPoison}  " +
                $"Burn: {playerBurn}";
        }

        if (enemyHpText != null && enemyUnit != null)
        {
            enemyHpText.text =
                $"Enemy HP: {enemyUnit.currentHp}/{enemyUnit.maxHp}  " +
                $"Block: {enemyUnit.currentBlock}  " +
                $"Poison: {enemyPoison}  " +
                $"Burn: {enemyBurn}";
        }

        if (energyText != null)
            energyText.text = $"Energy: {currentEnergy}/{maxEnergy}";

        if (stateText != null)
        {
            stateText.text =
                $"State: {state}\n" +
                $"Draw: {drawPile.Count}  Hand: {hand.Count}  Discard: {discardPile.Count}";
        }
    }

    private string GetRelicDisplayName(RelicType relicType)
    {
        switch (relicType)
        {
            case RelicType.VenomSac:
                return "Venom Sac";

            case RelicType.SmolderingAsh:
                return "Smoldering Ash";

            case RelicType.VolatileMixture:
                return "Volatile Mixture";

            default:
                return relicType.ToString();
        }
    }

    #endregion
}