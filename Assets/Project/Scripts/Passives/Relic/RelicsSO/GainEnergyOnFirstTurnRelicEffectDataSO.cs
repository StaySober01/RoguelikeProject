using UnityEngine;

[CreateAssetMenu(menuName = "Relics/Effects/Gain Energy On First Turn")]
public class GainEnergyOnFirstTurnRelicEffectDataSO : RelicEffectDataSO
{
    public int energyAmount = 1;

    public override IRelicEffect CreateRuntimeEffect()
    {
        return new GainEnergyOnFirstTurnRelicEffect(energyAmount);
    }
}
