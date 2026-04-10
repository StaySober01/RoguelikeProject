using UnityEngine;

public class FlameAccelerateEffect : ICardEffect
{
    public void Execute(EffectContext context)
    {
        int burnStack = context.Battle.statusEffectController.GetStack(
            context.Target,
            StatusEffectType.Burn);

        if (burnStack <= 0)
            return;

        context.Battle.DrawCards(burnStack);
    }
}
