using System;
using UnityEngine;

public class DealDamageDynamicEffect : ICardEffect
{
    private readonly Func<EffectContext, int> damageFunc;

    public DealDamageDynamicEffect(Func<EffectContext, int> damageFunc)
    {
        this.damageFunc = damageFunc;
    }

    public void Execute(EffectContext context)
    {
        int damage = damageFunc(context);

        if (damage > 0)
        {
            context.Battle.DealDamage(context.Source, context.Target, damage);
        }

        Debug.Log($"Deal {damage} dynamic damage.");
    }
}