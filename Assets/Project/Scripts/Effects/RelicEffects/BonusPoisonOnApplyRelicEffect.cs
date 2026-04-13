using UnityEngine;

public class BonusPoisonOnApplyRelicEffect : IRelicEffect
{
    private readonly int amount;

    public BonusPoisonOnApplyRelicEffect(int amount)
    {
        this.amount = amount;
    }

    public void OnEvent(RelicContext context)
    {
        if (context.TriggerType != RelicTriggerType.OnApplyPoison)
            return;

        if (context.Target == null)
            return;

        context.Battle.AddBattleLog($"Toxic Flask adds {amount} Poison");
        context.Battle.statusEffectController.ApplyPoison(
            context.Target,
            amount,
            false,
            false
        );
    }
}
