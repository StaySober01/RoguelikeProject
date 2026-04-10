using System.Collections.Generic;

public class SequenceEffect : ICardEffect
{
    private readonly List<ICardEffect> effects;

    public SequenceEffect(List<ICardEffect> effects)
    {
        this.effects = effects;
    }

    public void Execute(EffectContext context)
    {
        foreach (var effect in effects)
        {
            effect.Execute(context);
        }
    }
}