using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Draw Cards Effect Data")]
public class DrawCardsEffectDataSO : CardEffectDataSO
{
    public int amount;

    public override ICardEffect CreateRuntimeEffect()
    {
        return new DrawCardsEffect(amount);
    }
}
