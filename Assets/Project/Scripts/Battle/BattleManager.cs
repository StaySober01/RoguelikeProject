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
    private List<RelicType> rewardRelicChoices = new();
    public RelicManager relicManager;

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
    public TextMeshProUGUI battleText;

    [Header("Controllers")]
    public StatusEffectController statusEffectController;

    private CardEffectResolver cardEffectResolver = new();
    private int battleWinCount = 0;
    private RewardType currentRewardType;
    private bool isFirstPlayerTurnOfBattle = true;

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
        statusEffectController.Initialize(this);
    }

    private void Start()
    {
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
        if (chooseStartPassivePanel != null)
            chooseStartPassivePanel.SetActive(true);

        if (startPassiveButtons == null || startPassiveButtonTexts == null)
            return;

        StartPassiveType[] choices =
        {
            StartPassiveType.PoisonCore,
            StartPassiveType.BurnCore,
            StartPassiveType.VulnerableCore
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
        Debug.Log($"[Battle] Start passive selected: {passive}");
        AddStartPassiveCard(passive);

        if (chooseStartPassivePanel != null)
            chooseStartPassivePanel.SetActive(false);

        StartBattle();
    }

    private void AddStartPassiveCard(StartPassiveType passiveType)
    {
        switch (passiveType)
        {
            case StartPassiveType.PoisonCore:
                deck.Add(CardFactory.CreatePoison());
                break;

            case StartPassiveType.BurnCore:
                deck.Add(CardFactory.CreateBurn());
                break;

            case StartPassiveType.VulnerableCore:
                deck.Add(CardFactory.CreateVulnerable());
                break;
        }
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

        SetState(BattleState.PlayerTurn);

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
        RefreshUI();

        StartCoroutine(EnemyTurnRoutine());
    }

    public void OnClickEndTurn()
    {
        if (state != BattleState.PlayerTurn)
            return;

        Debug.Log("[Battle] Player turn end");

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
            ShowBattleReward();
            RefreshUI();
            return true;
        }

        if (playerUnit.IsDead())
        {
            SetState(BattleState.Lose);
            Debug.Log("[Battle] Defeat");
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
        Debug.Log($"[Card] Reward selected: {card.CardName}");

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
            RelicType.VolatileMixture,
            RelicType.PressurePoint,
            RelicType.OpeningSalvo,
            RelicType.QuickStart
        };

        relicPool.RemoveAll(relic => relicManager.HasRelic(relic));

        relicManager.ShuffleRelics(relicPool);

        int rewardCount = Mathf.Min(3, relicPool.Count);

        for (int i = 0; i < rewardCount; i++)
        {
            rewardRelicChoices.Add(relicPool[i]);
        }
    }

    public void SelectRelicReward(RelicType relicType)
    {
        relicManager.AddRelic(relicType);
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

    #endregion

    #region UI

    public void RefreshUI()
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
        int playerVulnerable = 0;
        int enemyPoison = 0;
        int enemyBurn = 0;
        int enemyVulnerable = 0;

        if (statusEffectController != null && playerUnit != null && enemyUnit != null)
        {
            playerPoison = statusEffectController.GetStack(playerUnit, StatusEffectType.Poison);
            playerBurn = statusEffectController.GetStack(playerUnit, StatusEffectType.Burn);
            playerVulnerable = statusEffectController.GetStack(playerUnit, StatusEffectType.Vulnerable);
            enemyPoison = statusEffectController.GetStack(enemyUnit, StatusEffectType.Poison);
            enemyBurn = statusEffectController.GetStack(enemyUnit, StatusEffectType.Burn);
            enemyVulnerable = statusEffectController.GetStack(enemyUnit, StatusEffectType.Vulnerable);
        }

        if (playerHpText != null && playerUnit != null)
        {
            playerHpText.text =
                $"Player HP: {playerUnit.currentHp}/{playerUnit.maxHp}  " +
                $"Block: {playerUnit.currentBlock}  " +
                $"Poison: {playerPoison}  " +
                $"Burn: {playerBurn}  " +
                $"Vulerable: {playerVulnerable}";
        }

        if (enemyHpText != null && enemyUnit != null)
        {
            enemyHpText.text =
                $"Enemy HP: {enemyUnit.currentHp}/{enemyUnit.maxHp}  " +
                $"Block: {enemyUnit.currentBlock}  " +
                $"Poison: {enemyPoison}  " +
                $"Burn: {enemyBurn}  " +
                $"Vulerable: {enemyVulnerable}";
        }

        if (energyText != null)
            energyText.text = $"Energy: {currentEnergy}/{maxEnergy}";

        if (stateText != null)
        {
            stateText.text =
                $"State: {state}\n" +
                $"Draw: {drawPile.Count}  Hand: {hand.Count}  Discard: {discardPile.Count}";
        }

        if (battleText != null)
            battleText.text = $"Battle {battleWinCount + 1}";
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

            case RelicType.PressurePoint:
                return "Pressure Point";

            case RelicType.OpeningSalvo:
                return "Opening Salvo";

            case RelicType.QuickStart:
                return "Quick Start";

            default:
                return relicType.ToString();
        }
    }

    #endregion
}
