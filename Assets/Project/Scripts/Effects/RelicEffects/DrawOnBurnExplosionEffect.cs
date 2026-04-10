public class DrawOnBurnExplosionEffect : IRelicEffect
{
    private int drawAmount;

    public DrawOnBurnExplosionEffect(int amount)
    {
        drawAmount = amount;
    }

    public void OnEvent(RelicContext context)
    {
        if (context.TriggerType != RelicTriggerType.OnBurnExploded)
            return;

        context.Battle.DrawCards(drawAmount);
    }
}