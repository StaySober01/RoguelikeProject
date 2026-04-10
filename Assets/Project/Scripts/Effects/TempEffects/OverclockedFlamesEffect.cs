using UnityEngine;

public class OverclockedFlamesEffect : ICardEffect
{
    public void Execute(EffectContext context)
    {
        context.Battle.EnableDoubleExplosionDamageThisTurn();
        Debug.Log("Overclocked Flames: Explosion damage is doubled this turn.");
    }
}