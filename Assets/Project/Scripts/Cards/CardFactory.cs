using System.Collections.Generic;
using UnityEngine;

public static class CardFactory
{
    public static CardInstance Create(CardDataSO data)
    {
        List<ICardEffect> runtimeEffects = new();

        foreach (var effectData in data.effectDataList)
        {
            if (effectData == null)
            {
                Debug.LogError($"[CardFactory] Null EffectData in {data.cardName}");
                continue;
            }

            var effect = effectData.CreateRuntimeEffect();

            if (effect == null)
            {
                Debug.LogError($"[CardFactory] Failed to create effect from {effectData.name}");
                continue;
            }

            runtimeEffects.Add(effect);
        }

        return new CardInstance(
            new CardData(
                data.cardId,
                data.cardName,
                data.category,
                data.cost,
                data.description,
                runtimeEffects,
                new List<CardTag>(data.tags)
            )
        );
    }
}