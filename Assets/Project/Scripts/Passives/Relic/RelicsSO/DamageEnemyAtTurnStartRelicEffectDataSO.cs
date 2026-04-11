using UnityEngine;

[CreateAssetMenu(menuName = "Relics/Effects/Damage Enemy At Turn Start")]
public class DamageEnemyAtTurnStartRelicEffectDataSO : RelicEffectDataSO
{
    public int damageAmount = 3;

    public override IRelicEffect CreateRuntimeEffect()
    {
        return new DamageEnemyAtTurnStartRelicEffect(damageAmount);
    }
}
