using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Special/Afterflare")]
public class AfterflareEffectDataSO : CardEffectDataSO
{
    public int burnAmount = 1;
    public int poisonAmountOnExplosion = 1;

    public override ICardEffect CreateRuntimeEffect()
    {
        return new AfterflareEffect(burnAmount, poisonAmountOnExplosion);
    }
}
