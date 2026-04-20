using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit
{
    [SerializeField] private List<EnemyActionData> actionPattern = new();
    private int currentPatternIndex = 0;

    private void OnEnable()
    {
        InitializeDefaultActionPatternIfEmpty();
    }

    public EnemyActionData GetCurrentAction()
    {
        if (actionPattern.Count == 0)
            return null;

        return actionPattern[currentPatternIndex];
    }

    public void AdvancePattern()
    {
        if (actionPattern.Count == 0)
            return;

        currentPatternIndex = (currentPatternIndex + 1) % actionPattern.Count;
    }

    private void InitializeDefaultActionPatternIfEmpty()
    {
        if (actionPattern.Count > 0)
            return;

        actionPattern.Add(new EnemyActionData
        {
            actionType = EnemyActionType.ApplyVulnerable,
            value = 2
        });

        actionPattern.Add(new EnemyActionData
        {
            actionType = EnemyActionType.Attack,
            value = 5
        });

        actionPattern.Add(new EnemyActionData
        {
            actionType = EnemyActionType.GainBlock,
            value = 6
        });

        currentPatternIndex = 0;
    }
}
