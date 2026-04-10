using System.Collections.Generic;
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
    public int Cost => data.cost;
    public string Description => data.description;
    public List<ICardEffect> Effects => data.effects;
    public List<CardTag> Tags => data.tags;
}