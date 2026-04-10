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
    }
}