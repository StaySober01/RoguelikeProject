using UnityEngine;

public class DrawOnBurnExplosionRelicEffect : IRelicEffect
{
    private readonly int drawAmount;

    public DrawOnBurnExplosionRelicEffect(int drawAmount)
    {
        this.drawAmount = drawAmount;
    }

    public void OnEvent(RelicContext context)
    {
        if (context.TriggerType != RelicTriggerType.OnBurnExploded)
            return;

        Debug.Log($"[Relic] Smoldering Ash triggers: draw {drawAmount}");
        context.Battle.DrawCards(drawAmount);
    }
}
