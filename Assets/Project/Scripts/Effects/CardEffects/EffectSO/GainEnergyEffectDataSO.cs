using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Gain Energy Effect Data")]
public class GainEnergyEffectDataSO : CardEffectDataSO
{
    public int amount;

    public override ICardEffect CreateRuntimeEffect()
    {
        return new GainEnergyEffect(amount);
    }
}
