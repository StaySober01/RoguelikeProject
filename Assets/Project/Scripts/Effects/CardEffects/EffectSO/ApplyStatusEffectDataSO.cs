using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Apply Status Effect Data")]
public class ApplyStatusEffectDataSO : CardEffectDataSO
{
    public StatusEffectType statusType;
    public int amount;

    public override ICardEffect CreateRuntimeEffect()
    {
        return new ApplyStatusEffect(statusType, amount);
    }
}