using System;

[Serializable]
public class CardEffectData
{
    public CardEffectType effectType;
    public int amount;

    public CardEffectData(CardEffectType effectType, int amount)
    {
        this.effectType = effectType;
        this.amount = amount;
    }
}