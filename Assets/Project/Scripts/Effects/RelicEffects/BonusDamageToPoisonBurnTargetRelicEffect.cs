public class BonusDamageToPoisonBurnTargetRelicEffect : IRelicEffect
{
    private readonly int bonusDamage;

    public BonusDamageToPoisonBurnTargetRelicEffect(int bonusDamage)
    {
        this.bonusDamage = bonusDamage;
    }

    public void OnEvent(RelicContext context)
    {
        if (context.TriggerType != RelicTriggerType.OnBeforeAttackResolved)
            return;

        if (context.Target == null)
            return;

        var controller = context.Battle.statusEffectController;

        bool hasPoison = controller.HasStatus(context.Target, StatusEffectType.Poison);
        bool hasBurn = controller.HasStatus(context.Target, StatusEffectType.Burn);

        if (!hasPoison || !hasBurn)
            return;

        int damage = context.Get<int>("damage");
        context.Set("damage", damage + bonusDamage);
    }
}