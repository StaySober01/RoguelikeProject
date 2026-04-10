using UnityEngine;

public class ToxicIgnitionEffect : ICardEffect
{
    public void Execute(EffectContext context)
    {
        bool isPoisoned = context.Battle.statusEffectController.HasStatus(
            context.Target,
            StatusEffectType.Poison);

        if (!isPoisoned)
        {
            Debug.Log("Toxic Ignition: Target is not Poisoned.");
            return;
        }

        context.Battle.AddRandomCardWithTagFromDrawPileToHand(CardTag.Burn);
        Debug.Log("Toxic Ignition: Target is Poisoned, add 1 random Burn-tagged card from draw pile to hand.");
    }
}