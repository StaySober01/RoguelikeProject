using System;

public class ConditionalEffect : ICardEffect
{
    private readonly Func<EffectContext, bool> condition;
    private readonly ICardEffect effect;

    public ConditionalEffect(Func<EffectContext, bool> condition, ICardEffect effect)
    {
        this.condition = condition;
        this.effect = effect;
    }

    public void Execute(EffectContext context)
    {
        if (condition(context))
        {
            effect.Execute(context);
        }
    }
}