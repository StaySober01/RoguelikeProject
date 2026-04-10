using UnityEngine;

public class AfterflareEffect : ICardEffect
{
    private readonly int burnAmount;
    private readonly int poisonAmountOnExplosion;

    public AfterflareEffect(int burnAmount = 1, int poisonAmountOnExplosion = 1)
    {
        this.burnAmount = burnAmount;
        this.poisonAmountOnExplosion = poisonAmountOnExplosion;
    }

    public void Execute(EffectContext context)
    {
        bool exploded = context.Battle.statusEffectController.ApplyBurn(
            context.Target,
            burnAmount);

        if (!exploded)
            return;

        context.Battle.statusEffectController.ApplyPoison(
            context.Target,
            poisonAmountOnExplosion);
    }
}
