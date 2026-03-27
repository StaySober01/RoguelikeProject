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
    public Button endTurnButton;

    [Header("Debug UI")]
    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI enemyHpText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI stateText;

    [Header("Energy")]
    public int maxEnergy = 3;
    private int currentEnergy; //나중에 상황 보고 다른 클래스로 옮기던지 할 것

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
        state = BattleState.PlayerTurn;
        currentEnergy = maxEnergy;

        Debug.Log($"Player Turn Start - Energy: {currentEnergy}/{maxEnergy}");

        SetPlayerActionButtons(true);
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

        if (currentEnergy < 1)
        {
            Debug.Log("Not enough energy to attack.");
            return;
        }

        StartCoroutine(PlayerAttackRoutine());
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

    private IEnumerator PlayerAttackRoutine()
    {
        state = BattleState.Busy;
        SetPlayerActionButtons(false);

        currentEnergy -= 1;
        Debug.Log($"Player attacks {enemyUnit.unitName} (Energy: {currentEnergy}/{maxEnergy})");

        enemyUnit.TakeDamage(playerUnit.attackPower);
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

        // 공격 후 턴 종료하지 않고, 다시 플레이어 선택으로 복귀
        state = BattleState.PlayerTurn;

        Debug.Log($"Player can act again. Remaining Energy: {currentEnergy}/{maxEnergy}");
        RefreshActionButtons();
        UpdateDebugUI();
    }

    private IEnumerator EnemyAttackRoutine()
    {
        state = BattleState.Busy;

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
        endTurnButton.interactable = isActive;
    }

    private void RefreshActionButtons()
    {
        if (state != BattleState.PlayerTurn)
        {
            SetPlayerActionButtons(false);
            return;
        }

        attackButton.interactable = currentEnergy >= 1;
        endTurnButton.interactable = true;
    }

    private void UpdateDebugUI()
    {
        if (playerHpText != null)
            playerHpText.text = $"Player HP: {playerUnit.currentHp}/{playerUnit.maxHp}";

        if (enemyHpText != null)
            enemyHpText.text = $"Enemy HP: {enemyUnit.currentHp}/{enemyUnit.maxHp}";

        if (energyText != null)
            energyText.text = $"Energy: {currentEnergy}/{maxEnergy}";

        if (stateText != null)
            stateText.text = $"State: {state}";
    }
}