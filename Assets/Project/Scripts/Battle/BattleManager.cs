using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
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
    public List<CardInstance> drawPile = new List<CardInstance>();
    public List<CardInstance> hand = new List<CardInstance>();
    public List<CardInstance> discardPile = new List<CardInstance>();

    [Header("UI - Hand")]
    public Button[] handButtons;
    public TextMeshProUGUI[] handButtonTexts;

    [Header("UI - Turn")]
    public Button endTurnButton;

    [Header("Debug UI")]
    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI enemyHpText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI stateText;

    [Header("Controllers")]
    public StatusEffectController statusEffectController;

    private void Start()
    {
        StartBattle();
    }

    #region Battle Flow

    public void StartBattle()
    {
        InitializeDeck();
        Debug.Log("Battle Start");
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        playerUnit.ResetBlock();
        currentEnergy = maxEnergy;
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

    #region Card System

    private void InitializeDeck()
    {
        drawPile = CardFactory.CreateStarterDeck();
        hand.Clear();
        discardPile.Clear();
        ShuffleCards(drawPile);
    }

    private void ShuffleCards(List<CardInstance> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            CardInstance temp = cards[i];
            int randomIndex = UnityEngine.Random.Range(i, cards.Count);
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

    private void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (hand.Count >= maxHandSize)
            {
                Debug.Log("Hand is full.");
                break;
            }

            if (drawPile.Count == 0)
            {
                ReshuffleDiscardIntoDrawPile();
            }

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

        ResolveCardEffect(card);

        yield return new WaitForSeconds(actionDelay);

        if (CheckBattleEnd())
            yield break;

        SetState(BattleState.PlayerTurn);
        Debug.Log($"Player action complete. Remaining Energy: {currentEnergy}/{maxEnergy}");
        RefreshUI();
    }

    private void ResolveCardEffect(CardInstance card)
    {
        foreach (CardEffectData effect in card.Effects)
        {
            switch (effect.effectType)
            {
                case CardEffectType.DealDamage:
                    enemyUnit.TakeDamage(effect.amount);
                    Debug.Log($"{card.CardName}: Deal {effect.amount} damage.");
                    break;

                case CardEffectType.GainBlock:
                    playerUnit.AddBlock(effect.amount);
                    Debug.Log($"{card.CardName}: Gain {effect.amount} Block.");
                    break;

                case CardEffectType.ApplyPoison:
                    statusEffectController.ApplyPoison(enemyUnit, effect.amount);
                    Debug.Log($"{card.CardName}: Apply {effect.amount} Poison.");
                    break;

                case CardEffectType.ApplyBurn:
                    statusEffectController.ApplyBurn(enemyUnit, effect.amount);
                    Debug.Log($"{card.CardName}: Apply {effect.amount} Burn.");
                    break;

                case CardEffectType.DrawCards:
                    DrawCards(effect.amount);
                    Debug.Log($"{card.CardName}: Draw {effect.amount} card(s).");
                    break;

                default:
                    Debug.LogWarning($"Unhandled card effect: {effect.effectType}");
                    break;
            }
        }

        ResolveSpecialCardLogic(card);
    }

    private void ResolveSpecialCardLogic(CardInstance card)
    {
        switch (card.CardId)
        {
            case "venom_guard":
                if (statusEffectController.HasStatus(enemyUnit, StatusEffectType.Poison))
                {
                    playerUnit.AddBlock(4);
                    Debug.Log($"{card.CardName}: Target was Poisoned, gain 4 Block.");
                }
                break;
        }
    }

    #endregion

    #region Battle Helpers

    private void SetState(BattleState newState)
    {
        state = newState;
        UpdateDebugUI();
    }

    private bool CheckBattleEnd()
    {
        if (enemyUnit.IsDead())
        {
            SetState(BattleState.Win);
            Debug.Log("Player Wins!");
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

    #endregion

    #region UI

    private void RefreshUI()
    {
        UpdateHandUI();

        if (endTurnButton != null)
            endTurnButton.interactable = (state == BattleState.PlayerTurn);

        UpdateDebugUI();
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

                bool canUse =
                    state == BattleState.PlayerTurn &&
                    currentEnergy >= card.Cost;

                handButtons[i].interactable = canUse;
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