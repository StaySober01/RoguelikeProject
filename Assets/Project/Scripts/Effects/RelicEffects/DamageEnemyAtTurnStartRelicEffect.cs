using UnityEngine;

public class DamageEnemyAtTurnStartRelicEffect : IRelicEffect
{
    private readonly int damageAmount;

    public DamageEnemyAtTurnStartRelicEffect(int damageAmount)
    {
        this.damageAmount = damageAmount;
    }

    public void OnEvent(RelicContext context)
    {
        if (context.TriggerType != RelicTriggerType.OnTurnStart)
            return;

        if (context.Target == null)
            return;

        Debug.Log($"[Relic] Opening Salvo triggers: {context.Target.unitName} takes {damageAmount} damage");
        context.Target.TakeDamage(damageAmount);
        context.Battle.AddBattleLog($"Opening Salvo deals {damageAmount} damage");
    }
}
