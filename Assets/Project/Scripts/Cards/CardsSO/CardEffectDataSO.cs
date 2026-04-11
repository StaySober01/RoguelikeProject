using UnityEngine;

public abstract class CardEffectDataSO : ScriptableObject
{
    public abstract ICardEffect CreateRuntimeEffect();
}