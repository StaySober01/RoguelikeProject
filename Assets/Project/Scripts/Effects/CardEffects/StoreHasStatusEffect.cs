public class StoreHasStatusEffect : ICardEffect
{
    private readonly string key;
    private readonly StatusEffectType statusType;

    public StoreHasStatusEffect(string key, StatusEffectType statusType)
    {
        this.key = key;
        this.statusType = statusType;
    }

    public void Execute(EffectContext context)
    {
        bool hasStatus = context.Battle.statusEffectController.HasStatus(
            context.Target, statusType);

        context.SetTemp(key, hasStatus);
    }
}