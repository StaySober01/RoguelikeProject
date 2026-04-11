using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Card Database")]
public class CardDatabaseSO : ScriptableObject
{
    [Header("Starter Deck")]
    public List<CardDataSO> starterDeckCards = new();

    [Header("Reward Pool")]
    public List<CardDataSO> rewardPoolCards = new();

    [Header("Start Passive Bonus Cards")]
    public List<StartPassiveCardEntry> startPassiveCards = new();

    public List<CardInstance> CreateStarterDeck()
    {
        List<CardInstance> starterDeck = new();

        foreach (CardDataSO cardData in starterDeckCards)
        {
            if (cardData == null)
                continue;

            CardInstance card = CardFactory.Create(cardData);
            if (card != null)
                starterDeck.Add(card);
        }

        return starterDeck;
    }

    public List<CardInstance> CreateRewardPool()
    {
        List<CardInstance> rewardPool = new();

        foreach (CardDataSO cardData in rewardPoolCards)
        {
            if (cardData == null)
                continue;

            CardInstance card = CardFactory.Create(cardData);
            if (card != null)
                rewardPool.Add(card);
        }

        return rewardPool;
    }

    public CardDataSO GetStartPassiveCard(StartPassiveType passiveType)
    {
        foreach (var entry in startPassiveCards)
        {
            if (entry.passiveType == passiveType)
                return entry.cardData;
        }

        return null;
    }
}