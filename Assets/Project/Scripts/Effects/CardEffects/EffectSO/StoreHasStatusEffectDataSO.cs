using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Store Has Status Effect Data")]
public class StoreHasStatusEffectDataSO : CardEffectDataSO
{
    public StatusEffectType statusType;
    public string tempKey;
    public bool checkTarget = true;

    public override ICardEffect CreateRuntimeEffect()
    {
        return new StoreHasStatusEffect(statusType, tempKey, checkTarget);
    }
}