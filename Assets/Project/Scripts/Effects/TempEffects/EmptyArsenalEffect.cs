using UnityEngine;

public class EmptyArsenalEffect : ICardEffect
{
    private readonly int drawAmount;

    public EmptyArsenalEffect(int drawAmount = 2)
    {
        this.drawAmount = drawAmount;
    }

    public void Execute(EffectContext context)
    {
        bool hasAttackCard = context.Battle.hand.Exists(
            card => card.Category == CardCategory.Attack);

        if (hasAttackCard)
        {
            Debug.Log("Empty Arsenal: Attack card exists in hand.");
            return;
        }

        context.Battle.DrawCards(drawAmount);
        Debug.Log($"Empty Arsenal: No Attack cards in hand, draw {drawAmount}.");
    }
}