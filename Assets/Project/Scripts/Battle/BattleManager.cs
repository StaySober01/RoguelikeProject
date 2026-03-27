using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public BattleState state;

    [Header("Units")]
    public SampleUnit playerUnit;
    public SampleUnit enemyUnit;

    [Header("UI")]
    public Button attackButton;

    private void Start()
    {
        StartBattle();
    }

    public void StartBattle()
    {
        state = BattleState.PlayerTurn;
        Debug.Log("Battle Start");
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        state = BattleState.PlayerTurn;
        Debug.Log("Player Turn Start");

        attackButton.interactable = true;
    }

    public void OnClickAttack()
    {
        if (state != BattleState.PlayerTurn)
            return;

        StartCoroutine(PlayerAttackRoutine());
    }

    private IEnumerator PlayerAttackRoutine()
    {
        state = BattleState.Busy;
        attackButton.interactable = false;

        Debug.Log($"Player attacks {enemyUnit.unitName}");
        enemyUnit.TakeDamage(playerUnit.attackPower);

        yield return new WaitForSeconds(0.5f);

        if (enemyUnit.IsDead())
        {
            state = BattleState.Win;
            Debug.Log("Player Wins!");
            yield break;
        }

        StartEnemyTurn();
    }

    public void StartEnemyTurn()
    {
        state = BattleState.EnemyTurn;
        Debug.Log("Enemy Turn Start");

        StartCoroutine(EnemyAttackRoutine());
    }

    private IEnumerator EnemyAttackRoutine()
    {
        state = BattleState.Busy;

        yield return new WaitForSeconds(1f);

        Debug.Log($"{enemyUnit.unitName} attacks Player");
        playerUnit.TakeDamage(enemyUnit.attackPower);

        yield return new WaitForSeconds(0.5f);

        if (playerUnit.IsDead())
        {
            state = BattleState.Lose;
            Debug.Log("Player Loses...");
            yield break;
        }

        Debug.Log("Turn End");
        StartPlayerTurn();
    }
}