public class StoreHasStatusEffect : ICardEffect
{
    private StatusEffectType statusType;
    private string tempKey;
    private bool checkTarget;

    public StoreHasStatusEffect(
        StatusEffectType statusType,
        string tempKey,
        bool checkTarget)
    {
        this.statusType = statusType;
        this.tempKey = tempKey;
        this.checkTarget = checkTarget;
    }

    public void Execute(EffectContext context)
    {
        var unit = checkTarget ? context.Target : context.Source;

        bool hasStatus = context.Battle.statusEffectController
            .HasStatus(unit, statusType);

        context.tempData[tempKey] = hasStatus;
    }
}