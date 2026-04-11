using UnityEngine;

[CreateAssetMenu(menuName = "Relics/Effects/Draw On Burn Explosion")]
public class DrawOnBurnExplosionRelicEffectDataSO : RelicEffectDataSO
{
    public int drawAmount = 1;

    public override IRelicEffect CreateRuntimeEffect()
    {
        return new DrawOnBurnExplosionRelicEffect(drawAmount);
    }
}
