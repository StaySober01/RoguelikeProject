using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    #region field

    public BattleState state;

    [Header("Units")]
    public Unit playerUnit;
    public Unit enemyUnit;

    [Header("UI Buttons")]
    public Button attackButton;
    public Button heavyAttackButton;
    public Button defendButton;
    public Button poisonButton;
    public Button burnButton;
    public Button endTurnButton;

    [Header("Debug UI")]
    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI enemyHpText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI stateText;

    [Header("Energy")]
    public int maxEnergy = 3;
    [SerializeField] private int currentEnergy;

    [Header("Action Values")]
    public int normalAttackCost = 1;
    public int heavyAttackCost = 2;
    public int defendCost = 1;
    public int poisonCost = 1;
    public int burnCost = 1;

    public int heavyAttackDamage = 10;
    public int defendBlockAmount = 6;

    [Header("Timings")]
    public float playerActionDelay = 0.5f;
    public float enemyActionDelay = 1.0f;
    public float enemyTurnEndDelay = 0.3f;

    [Header("Controllers")]
    public StatusEffectController statusEffectController;

    #endregion

    private void Start()
    {
        StartBattle();
    }

    #region Battle Flow

    public void StartBattle()
    {
        Debug.Log("Battle Start");
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        playerUnit.ResetBlock();
        currentEnergy = maxEnergy;
        SetState(BattleState.PlayerTurn);

        Debug.Log($"Player Turn Start - Energy: {currentEnergy}/{maxEnergy}");
        RefreshActionButtons();
    }

    public void StartEnemyTurn()
    {
        SetState(BattleState.EnemyTurn);
        Debug.Log("Enemy Turn Start");
        SetPlayerActionButtons(false);

        StartCoroutine(EnemyTurnRoutine());
    }

    public void OnClickEndTurn()
    {
        if (!CanPlayerAct())
            return;

        Debug.Log("Player ends turn");
        SetPlayerActionButtons(false);
        StartEnemyTurn();
    }

    #endregion

    #region Player Actions

    public void OnClickAttack()
    {
        TryStartPlayerAction(
            normalAttackCost,
            "Not enough energy for normal attack.",
            () =>
            {
                Debug.Log($"Player uses Attack on {enemyUnit.unitName}");
                enemyUnit.TakeDamage(playerUnit.attackPower);
            });
    }

    public void OnClickHeavyAttack()
    {
        TryStartPlayerAction(
            heavyAttackCost,
            "Not enough energy for heavy attack.",
            () =>
            {
                Debug.Log($"Player uses Heavy Attack on {enemyUnit.unitName}");
                enemyUnit.TakeDamage(heavyAttackDamage);
            });
    }

    public void OnClickDefend()
    {
        TryStartPlayerAction(
            defendCost,
            "Not enough energy to defend.",
            () =>
            {
                Debug.Log($"Player uses Defend");
                playerUnit.AddBlock(defendBlockAmount);
            });
    }

    public void OnClickPoison()
    {
        TryStartPlayerAction(
            poisonCost,
            "Not enough energy to use Poison.",
            () =>
            {
                Debug.Log("Player uses Poison");
                statusEffectController.ApplyPoison(enemyUnit, 1);
            });
    }

    public void OnClickBurn()
    {
        TryStartPlayerAction(
            burnCost,
            "Not enough energy to use Burn.",
            () =>
            {
                Debug.Log("Player uses Burn");
                statusEffectController.ApplyBurn(enemyUnit, 1);
            });
    }

    private void TryStartPlayerAction(int cost, string failLog, Action action)
    {
        if (!CanPlayerAct())
            return;

        if (!HasEnoughEnergy(cost))
        {
            Debug.Log(failLog);
            return;
        }

        StartCoroutine(PlayerActionRoutine(cost, action));
    }

    private IEnumerator PlayerActionRoutine(int cost, Action action)
    {
        SetState(BattleState.Busy);
        SetPlayerActionButtons(false);

        SpendEnergy(cost);
        action?.Invoke();
        UpdateDebugUI();

        yield return new WaitForSeconds(playerActionDelay);

        if (CheckBattleEnd())
            yield break;

        EndPlayerAction();
    }

    private void EndPlayerAction()
    {
        SetState(BattleState.PlayerTurn);
        Debug.Log($"Player action complete. Remaining Energy: {currentEnergy}/{maxEnergy}");
        RefreshActionButtons();
    }

    #endregion

    #region Enemy Turn

    private IEnumerator EnemyTurnRoutine()
    {
        SetState(BattleState.Busy);

        yield return new WaitForSeconds(enemyActionDelay);

        Debug.Log($"{enemyUnit.unitName} attacks Player");
        playerUnit.TakeDamage(enemyUnit.attackPower);
        UpdateDebugUI();

        if (CheckBattleEnd())
            yield break;

        yield return new WaitForSeconds(playerActionDelay);

        statusEffectController.ProcessTurnEnd(enemyUnit);

        yield return new WaitForSeconds(enemyTurnEndDelay);

        if (CheckBattleEnd())
            yield break;

        Debug.Log("Enemy Turn End");
        StartPlayerTurn();
    }

    #endregion

    #region Helpers

    private bool CanPlayerAct()
    {
        return state == BattleState.PlayerTurn;
    }

    private bool HasEnoughEnergy(int cost)
    {
        return currentEnergy >= cost;
    }

    private void SpendEnergy(int cost)
    {
        currentEnergy -= cost;
        Debug.Log($"Energy spent: {cost} -> {currentEnergy}/{maxEnergy}");
    }

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
            SetPlayerActionButtons(false);
            return true;
        }

        if (playerUnit.IsDead())
        {
            SetState(BattleState.Lose);
            Debug.Log("Player Loses...");
            SetPlayerActionButtons(false);
            return true;
        }

        return false;
    }

    #endregion

    #region UI

    private void SetPlayerActionButtons(bool isActive)
    {
        attackButton.interactable = isActive;
        heavyAttackButton.interactable = isActive;
        defendButton.interactable = isActive;
        poisonButton.interactable = isActive;
        burnButton.interactable = isActive;
        endTurnButton.interactable = isActive;
    }

    private void RefreshActionButtons()
    {
        if (state != BattleState.PlayerTurn)
        {
            SetPlayerActionButtons(false);
            return;
        }

        attackButton.interactable = HasEnoughEnergy(normalAttackCost);
        heavyAttackButton.interactable = HasEnoughEnergy(heavyAttackCost);
        defendButton.interactable = HasEnoughEnergy(defendCost);
        poisonButton.interactable = HasEnoughEnergy(poisonCost);
        burnButton.interactable = HasEnoughEnergy(burnCost);
        endTurnButton.interactable = true;

        UpdateDebugUI();
    }

    private void UpdateDebugUI()
    {
        int playerPoison = statusEffectController.GetStack(playerUnit, StatusEffectType.Poison);
        int playerBurn = statusEffectController.GetStack(playerUnit, StatusEffectType.Burn);

        int enemyPoison = statusEffectController.GetStack(enemyUnit, StatusEffectType.Poison);
        int enemyBurn = statusEffectController.GetStack(enemyUnit, StatusEffectType.Burn);

        if (playerHpText != null)
        {
            playerHpText.text =
                $"Player HP: {playerUnit.currentHp}/{playerUnit.maxHp}  " +
                $"Block: {playerUnit.currentBlock}  " +
                $"Poison: {playerPoison}  " +
                $"Burn: {playerBurn}";
        }

        if (enemyHpText != null)
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
            stateText.text = $"State: {state}";
    }

    #endregion
}