using System;

[Serializable]
public class CardInstance
{
    public CardData data;

    public CardInstance(CardData data)
    {
        this.data = data;
    }

    public string CardId => data.cardId;
    public string CardName => data.cardName;
    public CardCategory Category => data.category;
    public CardEffectType EffectType => data.effectType;
    public int Cost => data.cost;
    public int Amount => data.amount;
    public string Description => data.description;
}