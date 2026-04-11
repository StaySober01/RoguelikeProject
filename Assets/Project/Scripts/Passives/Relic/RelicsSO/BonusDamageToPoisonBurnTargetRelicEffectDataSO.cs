using UnityEngine;

[CreateAssetMenu(menuName = "Relics/Effects/Bonus Damage To Poison Burn Target")]
public class BonusDamageToPoisonBurnTargetRelicEffectDataSO : RelicEffectDataSO
{
    public int bonusDamage = 2;

    public override IRelicEffect CreateRuntimeEffect()
    {
        return new BonusDamageToPoisonBurnTargetRelicEffect(bonusDamage);
    }
}
