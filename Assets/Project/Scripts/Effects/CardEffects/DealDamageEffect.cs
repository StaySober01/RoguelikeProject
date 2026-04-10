public class DealDamageEffect : ICardEffect
{
    private readonly int amount;

    public DealDamageEffect(int amount)
    {
        this.amount = amount;
    }

    public void Execute(EffectContext context)
    {
        context.Battle.DealDamage(context.Source, context.Target, amount);
    }
}