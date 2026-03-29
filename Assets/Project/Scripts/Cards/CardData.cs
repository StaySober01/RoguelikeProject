using System;

[Serializable]
public class CardData
{
    public string cardId;
    public string cardName;
    public CardCategory category;
    public CardEffectType effectType;
    public int cost;
    public int amount;
    public string description;

    public CardData(
        string cardId,
        string cardName,
        CardCategory category,
        CardEffectType effectType,
        int cost,
        int amount,
        string description)
    {
        this.cardId = cardId;
        this.cardName = cardName;
        this.category = category;
        this.effectType = effectType;
        this.cost = cost;
        this.amount = amount;
        this.description = description;
    }
}