using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public BattleState state;

    [Header("Units")]
    public Unit playerUnit;
    public Unit enemyUnit;

    [Header("UI Buttons")]
    public Button attackButton;
    public Button heavyAttackButton;
    public Button defendButton;
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

    public int heavyAttackDamage = 10;
    public int defendBlockAmount = 6;

    private void Start()
    {
        StartBattle();
    }

    public void StartBattle()
    {
        Debug.Log("Battle Start");
        StartPlayerTurn();
        UpdateDebugUI();
    }

    public void StartPlayerTurn()
    {
        playerUnit.ResetBlock();

        state = BattleState.PlayerTurn;
        currentEnergy = maxEnergy;

        Debug.Log($"Player Turn Start - Energy: {currentEnergy}/{maxEnergy}");

        RefreshActionButtons();
        UpdateDebugUI();
    }

    public void StartEnemyTurn()
    {
        state = BattleState.EnemyTurn;
        Debug.Log("Enemy Turn Start");

        SetPlayerActionButtons(false);
        UpdateDebugUI();

        StartCoroutine(EnemyAttackRoutine());
    }

    public void OnClickAttack()
    {
        if (state != BattleState.PlayerTurn)
            return;

        if (currentEnergy < normalAttackCost)
        {
            Debug.Log("Not enough energy for normal attack.");
            return;
        }

        StartCoroutine(PlayerAttackRoutine(playerUnit.attackPower, normalAttackCost, "Attack"));
    }

    public void OnClickHeavyAttack()
    {
        if (state != BattleState.PlayerTurn)
            return;

        if (currentEnergy < heavyAttackCost)
        {
            Debug.Log("Not enough energy for heavy attack.");
            return;
        }

        StartCoroutine(PlayerAttackRoutine(heavyAttackDamage, heavyAttackCost, "Heavy Attack"));
    }

    public void OnClickDefend()
    {
        if (state != BattleState.PlayerTurn)
            return;

        if (currentEnergy < defendCost)
        {
            Debug.Log("Not enough energy to defend.");
            return;
        }

        StartCoroutine(PlayerDefendRoutine());
    }

    public void OnClickEndTurn()
    {
        if (state != BattleState.PlayerTurn)
            return;

        Debug.Log("Player ends turn");
        SetPlayerActionButtons(false);
        UpdateDebugUI();

        StartEnemyTurn();
    }

    private IEnumerator PlayerAttackRoutine(int damage, int cost, string actionName)
    {
        state = BattleState.Busy;
        SetPlayerActionButtons(false);

        currentEnergy -= cost;
        Debug.Log($"Player uses {actionName} on {enemyUnit.unitName} (Cost: {cost}, Energy: {currentEnergy}/{maxEnergy})");

        enemyUnit.TakeDamage(damage);
        UpdateDebugUI();

        yield return new WaitForSeconds(0.5f);

        if (enemyUnit.IsDead())
        {
            state = BattleState.Win;
            Debug.Log("Player Wins!");
            SetPlayerActionButtons(false);
            UpdateDebugUI();
            yield break;
        }

        state = BattleState.PlayerTurn;
        Debug.Log($"Player action complete. Remaining Energy: {currentEnergy}/{maxEnergy}");

        RefreshActionButtons();
        UpdateDebugUI();
    }

    private IEnumerator PlayerDefendRoutine()
    {
        state = BattleState.Busy;
        SetPlayerActionButtons(false);

        currentEnergy -= defendCost;
        Debug.Log($"Player uses Defend (Cost: {defendCost}, Energy: {currentEnergy}/{maxEnergy})");

        playerUnit.AddBlock(defendBlockAmount);
        UpdateDebugUI();

        yield return new WaitForSeconds(0.5f);

        state = BattleState.PlayerTurn;
        Debug.Log($"Player action complete. Remaining Energy: {currentEnergy}/{maxEnergy}");

        RefreshActionButtons();
        UpdateDebugUI();
    }

    private IEnumerator EnemyAttackRoutine()
    {
        state = BattleState.Busy;
        UpdateDebugUI();

        yield return new WaitForSeconds(1f);

        Debug.Log($"{enemyUnit.unitName} attacks Player");
        playerUnit.TakeDamage(enemyUnit.attackPower);
        UpdateDebugUI();

        yield return new WaitForSeconds(0.5f);

        if (playerUnit.IsDead())
        {
            state = BattleState.Lose;
            Debug.Log("Player Loses...");
            SetPlayerActionButtons(false);
            UpdateDebugUI();
            yield break;
        }

        Debug.Log("Enemy Turn End");
        StartPlayerTurn();
    }

    private void SetPlayerActionButtons(bool isActive)
    {
        attackButton.interactable = isActive;
        heavyAttackButton.interactable = isActive;
        defendButton.interactable = isActive;
        endTurnButton.interactable = isActive;
    }

    private void RefreshActionButtons()
    {
        if (state != BattleState.PlayerTurn)
        {
            SetPlayerActionButtons(false);
            return;
        }

        attackButton.interactable = currentEnergy >= normalAttackCost;
        heavyAttackButton.interactable = currentEnergy >= heavyAttackCost;
        defendButton.interactable = currentEnergy >= defendCost;
        endTurnButton.interactable = true;
    }

    private void UpdateDebugUI()
    {
        if (playerHpText != null)
            playerHpText.text = $"Player HP: {playerUnit.currentHp}/{playerUnit.maxHp}  Block: {playerUnit.currentBlock}";

        if (enemyHpText != null)
            enemyHpText.text = $"Enemy HP: {enemyUnit.currentHp}/{enemyUnit.maxHp}  Block: {enemyUnit.currentBlock}";

        if (energyText != null)
            energyText.text = $"Energy: {currentEnergy}/{maxEnergy}";

        if (stateText != null)
            stateText.text = $"State: {state}";
    }
}