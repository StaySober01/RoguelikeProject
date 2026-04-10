public class DamageEnemyAtTurnStartRelicEffect : IRelicEffect
{
    private readonly int damageAmount;

    public DamageEnemyAtTurnStartRelicEffect(int damageAmount)
    {
        this.damageAmount = damageAmount;
    }

    public void OnEvent(RelicContext context)
    {
        if (context.TriggerType != RelicTriggerType.OnTurnStart)
            return;

        if (context.Target == null)
            return;

        context.Target.TakeDamage(damageAmount);
    }
}