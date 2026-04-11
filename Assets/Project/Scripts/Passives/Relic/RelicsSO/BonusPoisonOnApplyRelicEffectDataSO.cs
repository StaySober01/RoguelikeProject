using UnityEngine;

[CreateAssetMenu(menuName = "Relics/Effects/Bonus Poison On Apply")]
public class BonusPoisonOnApplyRelicEffectDataSO : RelicEffectDataSO
{
    public int amount = 1;

    public override IRelicEffect CreateRuntimeEffect()
    {
        return new BonusPoisonOnApplyRelicEffect(amount);
    }
}
