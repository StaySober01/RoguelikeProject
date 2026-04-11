using UnityEngine;

[CreateAssetMenu(menuName = "Relics/Effects/Gain Energy On Attack Vulnerable Target")]
public class GainEnergyOnAttackVulnerableTargetRelicEffectDataSO : RelicEffectDataSO
{
    public int energyAmount = 1;

    public override IRelicEffect CreateRuntimeEffect()
    {
        return new GainEnergyOnAttackVulnerableTargetRelicEffect(energyAmount);
    }
}
