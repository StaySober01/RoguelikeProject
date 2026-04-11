using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Special/Toxic Ignition")]
public class ToxicIgnitionEffectDataSO : CardEffectDataSO
{
    public override ICardEffect CreateRuntimeEffect()
    {
        return new ToxicIgnitionEffect();
    }
}
