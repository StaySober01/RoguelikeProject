using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private List<RelicDataSO> rewardRelicChoices = new();
    public RelicManager relicManager;

    [Header("UI")]
    [SerializeField] private BattleUIManager battleUIManager;

    [Header("Controllers")]
    public StatusEffectController statusEffectController;

    private CardEffectResolver cardEffectResolver = new();
    private int battleWinCount = 0;
    private RewardType currentRewardType;
    private bool isFirstPlayerTurnOfBattle = true;
    private bool gainedEnergyFromPressurePointThisTurn = false;

    [SerializeField] private CardDatabaseSO cardDatabase;
    [SerializeField] private RelicDatabaseSO relicDatabase;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[Battle] Duplicate BattleManager detected. Destroying this instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        EnsureBattleUIManager();
        statusEffectController.Initialize(this);
    }

    private void Start()
    {
        EnsureBattleUIManager();
        relicManager.Initialize(this, statusEffectController);
        baseEnemyHp = enemyUnit.maxHp;
        baseEnemyAttack = enemyUnit.attackPower;
        InitializePermanentDeck();
        ShowStartPassiveSelection();
    }

    #endregion

    #region Start Passive Selection

    private void ShowStartPassiveSelection()
    {
        StartPassiveType[] choices =
        {
            StartPassiveType.PoisonCore,
            StartPassiveType.BurnCore,
            StartPassiveType.VulnerableCore
        };
        battleUIManager?.ShowStartPassiveSelection(choices, SelectStartPassive);
    }

    public void SelectStartPassive(StartPassiveType passive)
    {
        playerUnit.selectedStartPassive = passive;
        Debug.Log($"[Battle] Start passive selected: {passive}");
        AddStartPassiveCard(passive);

        battleUIManager?.HideStartPassiveSelection();

        StartBattle();
    }

    private void AddStartPassiveCard(StartPassiveType passiveType)
    {
        CardDataSO cardData = cardDatabase.GetStartPassiveCard(passiveType);

        if (cardData == null)
        {
            Debug.LogError($"[Battle] No start passive card assigned for {passiveType}");
            return;
        }

        CardInstance card = CardFactory.Create(cardData);

        if (card == null)
        {
            Debug.LogError($"[Battle] Failed to create start passive card for {passiveType}");
            return;
        }

        deck.Add(card);
    }

    public bool HasStartPassive(StartPassiveType startPassiveType)
    {
        return playerUnit.selectedStartPassive == startPassiveType;
    }

    #endregion

    #region Battle Flow

    public void StartBattle()
    {
        isFirstPlayerTurnOfBattle = true;
        InitializeBattleDeck();
        ScaleEnemyForBattle();
        Debug.Log($"[Battle] Battle {battleWinCount + 1} start");
        //ClearBattleLogs();
        AddBattleLog($"Battle {battleWinCount + 1} start");

        TriggerBattleStartRelics();

        if (CheckBattleEnd())
            return;

        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        playerUnit.ResetBlock();
        currentEnergy = maxEnergy;
        statusEffectController.burnExplosionDamageMultiplier = 1;
        gainedEnergyFromPressurePointThisTurn = false;

        SetState(BattleState.PlayerTurn);

        AddBattleLog("Player Turn Start");

        TriggerTurnStartRelics();

        if (CheckBattleEnd())
            return;

        DiscardHand();
        DrawCards(drawCountPerTurn);

        Debug.Log($"[Battle] Player turn start - Energy: {currentEnergy}/{maxEnergy}");
        RefreshUI();

        isFirstPlayerTurnOfBattle = false;
    }

    public void StartEnemyTurn()
    {
        SetState(BattleState.EnemyTurn);
        Debug.Log("[Battle] Enemy turn start");
        AddBattleLog("Enemy Turn Start");
        RefreshUI();

        StartCoroutine(EnemyTurnRoutine());
    }

    public void OnClickEndTurn()
    {
        if (state != BattleState.PlayerTurn)
            return;

        Debug.Log("[Battle] Player turn end");

        AddBattleLog("Turn End");

        TriggerTurnEndRelics();

        if (CheckBattleEnd())
            return;

        RefreshUI();
        StartEnemyTurn();
    }

    private bool CheckBattleEnd()
    {
        if (enemyUnit.IsDead())
        {
            SetState(BattleState.Win);
            battleWinCount++;
            Debug.Log("[Battle] Victory");
            AddBattleLog("Victory");
            ShowBattleReward();
            RefreshUI();
            return true;
        }

        if (playerUnit.IsDead())
        {
            SetState(BattleState.Lose);
            Debug.Log("[Battle] Defeat");
            AddBattleLog("Defeat");
            RefreshUI();
            return true;
        }

        return false;
    }

    private void ScaleEnemyForBattle()
    {
        enemyUnit.maxHp = baseEnemyHp + (battleWinCount * 3);
        enemyUnit.currentHp = enemyUnit.maxHp;
        enemyUnit.attackPower = baseEnemyAttack + (battleWinCount / 2);
    }

    private void SetState(BattleState newState)
    {
        state = newState;
        UpdateDebugUI();
    }

    public void DealDamage(Unit attacker, Unit target, int baseDamage)
    {
        int damageAfterStatus = statusEffectController.GetDamageWithVulnerable(target, baseDamage);

        RelicContext context = relicManager.CreateContext(attacker, target);
        context.Set("damage", damageAfterStatus);

        relicManager.Trigger(RelicTriggerType.OnBeforeAttackResolved, context);

        int finalDamage = context.Get<int>("damage");
        target.TakeDamage(finalDamage);

        relicManager.Trigger(RelicTriggerType.OnAfterAttackResolved, context);

        RefreshUI();
    }

    #endregion

    #region Enemy Turn

    private IEnumerator EnemyTurnRoutine()
    {
        SetState(BattleState.Busy);
        RefreshUI();

        yield return new WaitForSeconds(enemyAttackDelay);

        Debug.Log($"[Battle] {enemyUnit.unitName} attacks {playerUnit.unitName}");
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

        Debug.Log("[Battle] Enemy turn end");
        StartPlayerTurn();
    }

    #endregion

    #region Reward Flow

    private void GenerateCardRewards()
    {
        rewardCardChoices.Clear();

        List<CardInstance> rewardPool = cardDatabase.CreateRewardPool();
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
        Debug.Log($"[Card] Reward selected: {card.CardName}");
        AddBattleLog($"Card reward selected: {card.CardName}");

        HideRewardUI();
        StartNextBattle();
    }

    private void GenerateRelicRewards()
    {
        rewardRelicChoices.Clear();

        if (relicDatabase == null)
        {
            Debug.LogError("[Battle] RelicDatabase is not assigned.");
            return;
        }

        List<RelicDataSO> relicPool = relicDatabase.CreateRewardPool(relicManager);

        relicManager.ShuffleRelics(relicPool);

        int rewardCount = Mathf.Min(3, relicPool.Count);

        for (int i = 0; i < rewardCount; i++)
        {
            rewardRelicChoices.Add(relicPool[i]);
        }
    }

    public void SelectRelicReward(RelicDataSO relicData)
    {
        if (relicData == null)
        {
            Debug.LogError("[Battle] Tried to select a null relic reward.");
            return;
        }

        relicManager.AddRelic(relicData);
        AddBattleLog($"Relic reward selected: {relicData.DisplayName}");
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
        Debug.Log("[UI] Reward skipped");

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
        deck = cardDatabase.CreateStarterDeck();
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
            return;

        drawPile.AddRange(discardPile);
        discardPile.Clear();
        ShuffleCards(drawPile);
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (hand.Count >= maxHandSize)
                break;

            if (drawPile.Count == 0)
                ReshuffleDiscardIntoDrawPile();

            if (drawPile.Count == 0)
                break;

            CardInstance drawnCard = drawPile[0];
            drawPile.RemoveAt(0);
            hand.Add(drawnCard);
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
            Debug.Log($"[Card] Cannot use {card.CardName} - insufficient energy ({currentEnergy}/{maxEnergy})");
            return;
        }

        StartCoroutine(PlayerUseCardRoutine(card));
    }

    private IEnumerator PlayerUseCardRoutine(CardInstance card)
    {
        SetState(BattleState.Busy);
        RefreshUI();

        currentEnergy -= card.Cost;
        Debug.Log($"[Card] Used {card.CardName} (Cost: {card.Cost}, Energy: {currentEnergy}/{maxEnergy})");
        AddBattleLog($"{card.CardName} used");

        hand.Remove(card);
        discardPile.Add(card);

        UpdateHandUI();
        UpdateDebugUI();

        EffectContext context = new EffectContext(this, playerUnit, enemyUnit, card);
        cardEffectResolver.Resolve(context);

        yield return new WaitForSeconds(actionDelay);

        if (CheckBattleEnd())
            yield break;

        SetState(BattleState.PlayerTurn);
        RefreshUI();
    }

    public void AddRandomCardWithTagFromDrawPileToHand(CardTag tag)
    {
        List<CardInstance> candidates = drawPile.FindAll(card => card.Tags.Contains(tag));

        if (candidates.Count == 0)
            return;

        if (hand.Count >= maxHandSize)
            return;

        CardInstance selectedCard = candidates[Random.Range(0, candidates.Count)];

        drawPile.Remove(selectedCard);
        hand.Add(selectedCard);

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
        statusEffectController.burnExplosionDamageMultiplier = 2;
    }

    #endregion

    #region Relic System

    private void TriggerBattleStartRelics()
    {
        RelicContext context = relicManager.CreateContext(playerUnit, enemyUnit);
        relicManager.Trigger(RelicTriggerType.OnBattleStart, context);
        RefreshUI();
    }

    private void TriggerTurnStartRelics()
    {
        RelicContext context = relicManager.CreateContext(playerUnit, enemyUnit);
        context.Set("isFirstTurn", isFirstPlayerTurnOfBattle);

        relicManager.Trigger(RelicTriggerType.OnTurnStart, context);
        RefreshUI();
    }

    private void TriggerTurnEndRelics()
    {
        RelicContext context = relicManager.CreateContext(playerUnit, enemyUnit);
        relicManager.Trigger(RelicTriggerType.OnTurnEnd, context);
        RefreshUI();
    }

    public bool CanGainPressurePointEnergy()
    {
        return !gainedEnergyFromPressurePointThisTurn;
    }

    public void MarkPressurePointEnergyGained()
    {
        gainedEnergyFromPressurePointThisTurn = true;
    }

    #endregion

    #region Getter
    public IReadOnlyList<CardInstance> GetDrawPile()
    {
        return drawPile;
    }

    public IReadOnlyList<CardInstance> GetDiscardPile()
    {
        return discardPile;
    }

    public IReadOnlyList<CardInstance> GetCurrentDeck()
    {
        return deck;
    }

    #endregion

    #region UI

    public void RefreshUI()
    {
        battleUIManager?.RefreshUI(
            hand,
            state,
            currentEnergy,
            maxEnergy,
            drawPile.Count,
            hand.Count,
            discardPile.Count,
            battleWinCount,
            playerUnit,
            enemyUnit,
            statusEffectController,
            UseCard);
    }

    private void ShowCardRewardUI()
    {
        battleUIManager?.ShowCardRewardUI(rewardCardChoices, SelectRewardCard, SkipReward);
    }

    private void ShowRelicRewardUI()
    {
        battleUIManager?.ShowRelicRewardUI(rewardRelicChoices, SelectRelicReward, SkipReward);
    }

    private void HideRewardUI()
    {
        rewardCardChoices.Clear();
        rewardRelicChoices.Clear();
        battleUIManager?.HideRewardUI();
    }

    private void UpdateHandUI()
    {
        battleUIManager?.UpdateHandUI(hand, state, currentEnergy, UseCard);
    }

    private void UpdateDebugUI()
    {
        battleUIManager?.UpdateDebugUI(
            state,
            currentEnergy,
            maxEnergy,
            drawPile.Count,
            hand.Count,
            discardPile.Count,
            battleWinCount,
            playerUnit,
            enemyUnit,
            statusEffectController);
    }

    public void AddBattleLog(string message)
    {
        battleUIManager?.AddBattleLog(message);
    }

    public void ClearBattleLogs()
    {
        battleUIManager?.ClearBattleLogs();
    }

    public string GetBattleLogUnitName(Unit unit)
    {
        if (unit == null)
            return "Target";

        if (unit == playerUnit)
            return "Player";

        if (unit == enemyUnit)
            return "Enemy";

        return string.IsNullOrWhiteSpace(unit.unitName) ? "Target" : unit.unitName;
    }

    #endregion

    private void EnsureBattleUIManager()
    {
        if (battleUIManager == null)
            battleUIManager = GetComponent<BattleUIManager>();

        if (battleUIManager == null)
            battleUIManager = gameObject.AddComponent<BattleUIManager>();
    }
}
