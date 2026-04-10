using UnityEngine;

public class FlameAccelerateEffect : ICardEffect
{
    public void Execute(EffectContext context)
    {
        int burnStack = context.Battle.statusEffectController.GetStack(
            context.Target,
            StatusEffectType.Burn);

        if (burnStack <= 0)
        {
            Debug.Log("Flame Accelerate: Target has no Burn.");
            return;
        }

        context.Battle.DrawCards(burnStack);
        Debug.Log($"Flame Accelerate: Draw {burnStack} card(s) equal to target's Burn stacks.");
    }
}