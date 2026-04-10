using UnityEngine;

public class GainEnergyOnAttackVulnerableTargetRelicEffect : IRelicEffect
{
    private readonly int energyAmount;

    public GainEnergyOnAttackVulnerableTargetRelicEffect(int energyAmount)
    {
        this.energyAmount = energyAmount;
    }

    public void OnEvent(RelicContext context)
    {
        if (context.TriggerType != RelicTriggerType.OnAfterAttackResolved)
            return;

        if (context.Target == null)
            return;

        bool hasVulnerable = context.Battle.statusEffectController.HasStatus(
            context.Target,
            StatusEffectType.Vulnerable);

        if (!hasVulnerable)
            return;

        Debug.Log($"[Relic] Pressure Point triggers: gain {energyAmount} Energy");
        context.Battle.GainEnergy(energyAmount);
    }
}
