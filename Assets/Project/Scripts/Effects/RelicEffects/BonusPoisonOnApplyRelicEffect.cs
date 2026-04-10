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

        if (context.Get<bool>("isBonusApplied"))
            return;

        context.Set("isBonusApplied", true);
        Debug.Log($"[Relic] Venom Sac triggers: +{amount} Poison");
        context.Battle.statusEffectController.ApplyPoison(context.Target, amount);
    }
}
