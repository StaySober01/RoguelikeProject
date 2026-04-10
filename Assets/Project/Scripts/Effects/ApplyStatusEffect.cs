using UnityEngine;

public class ApplyStatusEffect : ICardEffect
{
    private readonly StatusEffectType statusType;
    private readonly int amount;

    public ApplyStatusEffect(StatusEffectType statusType, int amount)
    {
        this.statusType = statusType;
        this.amount = amount;
    }

    public void Execute(EffectContext context)
    {
        switch (statusType)
        {
            case StatusEffectType.Poison:
                context.Battle.statusEffectController.ApplyPoison(context.Target, amount);
                break;

            case StatusEffectType.Burn:
                context.Battle.statusEffectController.ApplyBurn(context.Target, amount);
                break;

            case StatusEffectType.Vulnerable:
                context.Battle.statusEffectController.ApplyVulnerable(context.Target, amount);
                break;

            default:
                Debug.LogWarning($"Unhandled status type: {statusType}");
                break;
        }
    }
}