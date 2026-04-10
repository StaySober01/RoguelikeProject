using System.Collections.Generic;
using System;

[Serializable]
public class CardData
{
    public string cardId;
    public string cardName;
    public CardCategory category;
    public int cost;
    public string description;
    public List<ICardEffect> effects;
    public List<CardTag> tags;

    public CardData(
        string cardId,
        string cardName,
        CardCategory category,
        int cost,
        string description,
        List<ICardEffect> effects,
        List<CardTag> tags = null)
    {
        this.cardId = cardId;
        this.cardName = cardName;
        this.category = category;
        this.cost = cost;
        this.description = description;
        this.effects = effects;
        this.tags = tags ?? new List<CardTag>();
    }
}