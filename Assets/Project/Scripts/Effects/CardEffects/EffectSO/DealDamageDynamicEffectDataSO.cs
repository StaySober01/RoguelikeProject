using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Dynamic Deal Damage Effect Data")]
public class DealDamageDynamicEffectDataSO : CardEffectDataSO
{
    public ValueProviderDataSO valueProvider;

    public override ICardEffect CreateRuntimeEffect()
    {
        return new DealDamageDynamicEffect(
            context => valueProvider != null ? valueProvider.GetValue(context) : 0
        );
    }
}