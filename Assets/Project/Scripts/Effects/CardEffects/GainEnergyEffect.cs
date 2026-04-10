public class GainEnergyEffect : ICardEffect
{
    private readonly int amount;

    public GainEnergyEffect(int amount)
    {
        this.amount = amount;
    }

    public void Execute(EffectContext context)
    {
        context.Battle.GainEnergy(amount);
    }
}