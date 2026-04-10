using UnityEngine;

public class ToxicIgnitionEffect : ICardEffect
{
    public void Execute(EffectContext context)
    {
        bool isPoisoned = context.Battle.statusEffectController.HasStatus(
            context.Target,
            StatusEffectType.Poison);

        if (!isPoisoned)
            return;

        context.Battle.AddRandomCardWithTagFromDrawPileToHand(CardTag.Burn);
    }
}
