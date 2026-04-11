using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Card Data")]
public class CardDataSO : ScriptableObject
{
    public string cardId;
    public string cardName;
    [TextArea] public string description;
    public int cost;

    public CardCategory category;
    public List<CardTag> tags = new();
    public List<CardEffectDataSO> effectDataList = new();
}