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

    [Header("Card System")]
    public int drawCountPerTurn = 5;
    public int maxHandSize = 5;
    public List<CardInstance> drawPile = new();
    public List<CardInstance> hand = new();
    public List<CardInstance> discardPile = new();
    public List<CardInstance> deck = new();
    private List<CardInstance> rewardChoices = new();

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
            Debug.Log("Player Wins!");
            GenerateCardRewards();
            ShowRewardUI();
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
        rewardChoices.Clear();

        List<CardInstance> rewardPool = CardFactory.CreateRewardCardPool();
        ShuffleCards(rewardPool);

        int rewardCount = Mathf.Min(3, rewardPool.Count);

        for (int i = 0; i < rewardCount; i++)
        {
            rewardChoices.Add(rewardPool[i]);
        }
    }

    public void SelectRewardCard(CardInstance card)
    {
        deck.Add(card);
        Debug.Log($"{card.CardName} added to permanent deck.");

        HideRewardUI();
        StartNextBattle();
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

    #region UI

    private void RefreshUI()
    {
        UpdateHandUI();

        if (endTurnButton != null)
            endTurnButton.interactable = (state == BattleState.PlayerTurn);

        UpdateDebugUI();
    }

    private void ShowRewardUI()
    {
        if (rewardPanel != null)
            rewardPanel.SetActive(true);

        if (rewardButtons == null || rewardButtonTexts == null)
            return;

        for (int i = 0; i < rewardButtons.Length; i++)
        {
            if (i < rewardChoices.Count)
            {
                CardInstance card = rewardChoices[i];

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

    private void HideRewardUI()
    {
        rewardChoices.Clear();

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

    #endregion
}