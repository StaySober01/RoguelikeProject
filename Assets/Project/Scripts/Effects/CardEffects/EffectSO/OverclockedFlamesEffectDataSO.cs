using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Special/Overclocked Flames")]
public class OverclockedFlamesEffectDataSO : CardEffectDataSO
{
    public override ICardEffect CreateRuntimeEffect()
    {
        return new OverclockedFlamesEffect();
    }
}
