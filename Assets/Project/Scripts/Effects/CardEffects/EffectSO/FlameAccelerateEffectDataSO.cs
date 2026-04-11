using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Special/Flame Accelerate")]
public class FlameAccelerateEffectDataSO : CardEffectDataSO
{
    public override ICardEffect CreateRuntimeEffect()
    {
        return new FlameAccelerateEffect();
    }
}
