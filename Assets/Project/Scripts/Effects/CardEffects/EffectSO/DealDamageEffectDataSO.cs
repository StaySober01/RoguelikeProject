using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Deal Damage Effect Data")]
public class DealDamageEffectDataSO : CardEffectDataSO
{
    public int damage;

    public override ICardEffect CreateRuntimeEffect()
    {
        return new DealDamageEffect(damage);
    }
}