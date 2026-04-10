using UnityEngine;

public class OverclockedFlamesEffect : ICardEffect
{
    public void Execute(EffectContext context)
    {
        context.Battle.EnableDoubleExplosionDamageThisTurn();
    }
}
