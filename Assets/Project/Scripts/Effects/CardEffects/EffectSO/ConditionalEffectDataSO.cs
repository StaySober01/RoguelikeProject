using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Conditional Effect Data")]
public class ConditionalEffectDataSO : CardEffectDataSO
{
    public ConditionDataSO condition;
    public List<CardEffectDataSO> trueEffects = new();

    public override ICardEffect CreateRuntimeEffect()
    {
        List<ICardEffect> runtimeTrueEffects = new();

        foreach (var effectData in trueEffects)
        {
            if (effectData != null)
                runtimeTrueEffects.Add(effectData.CreateRuntimeEffect());
        }

        return new ConditionalEffect(condition, runtimeTrueEffects);
    }
}