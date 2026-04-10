using System;
using UnityEngine;
using UnityEngine.XR;

public class CardEffectResolver
{
    public void Resolve(EffectContext context)
    {
        foreach (var effect in context.Card.Effects)
        {
            effect.Execute(context);
        }
    }
}