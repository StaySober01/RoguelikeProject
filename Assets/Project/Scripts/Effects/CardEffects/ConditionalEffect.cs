using System;
using System.Collections.Generic;

public class ConditionalEffect : ICardEffect
{
    private ConditionDataSO condition;
    private List<ICardEffect> trueEffects;

    public ConditionalEffect(
        ConditionDataSO condition,
        List<ICardEffect> trueEffects)
    {
        this.condition = condition;
        this.trueEffects = trueEffects;
    }

    public void Execute(EffectContext context)
    {
        if (condition != null && condition.Evaluate(context))
        {
            foreach (var effect in trueEffects)
            {
                effect.Execute(context);
            }
        }
    }
}