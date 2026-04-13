public class DrawCardsEffect : ICardEffect
{
    private readonly int amount;

    public DrawCardsEffect(int amount)
    {
        this.amount = amount;
    }

    public void Execute(EffectContext context)
    {
        context.Battle.DrawCards(amount);
        context.Battle.AddBattleLog($"Draw {amount} card{(amount == 1 ? string.Empty : "s")}");
    }
}
