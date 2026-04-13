public class GainBlockEffect : ICardEffect
{
    private readonly int amount;

    public GainBlockEffect(int amount)
    {
        this.amount = amount;
    }

    public void Execute(EffectContext context)
    {
        context.Source.AddBlock(amount);
        context.Battle.AddBattleLog($"{context.Battle.GetBattleLogUnitName(context.Source)} gains {amount} Block");
    }
}
