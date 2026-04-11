using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Gain Block Effect Data")]
public class GainBlockEffectDataSO : CardEffectDataSO
{
    public int amount;

    public override ICardEffect CreateRuntimeEffect()
    {
        return new GainBlockEffect(amount);
    }
}
