using UnityEngine;

public class StatusEffectController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BattleManager battleManager;

    [Header("Burn")]
    public int burnExplosionDamage = 10;
    public int burnExplosionDamageMultiplier = 1;

    public void Initialize(BattleManager battleManager)
    {
        this.battleManager = battleManager;
    }

    public void ApplyPoison(Unit target, int amount)
    {
        if (target == null || amount <= 0)
            return;

        int finalAmount = amount;

        if (battleManager.HasStartPassive(StartPassiveType.PoisonCore))
        {
            finalAmount += 1;
            Debug.Log("Start Passive triggered: Poison Core (+1 Poison)");
        }

        target.statusData.AddStack(StatusEffectType.Poison, finalAmount);
        Debug.Log($"{target.unitName} gains {finalAmount} Poison.");

        battleManager.relicManager.Trigger(
            RelicTriggerType.OnApplyPoison,
            battleManager.relicManager.CreateContext(source: null, target: target)
        );

        battleManager.RefreshUI();
    }

    public bool ApplyBurn(Unit target, int amount)
    {
        if (target == null || amount <= 0)
            return false;

        target.statusData.AddStack(StatusEffectType.Burn, amount);
        Debug.Log($"{target.unitName} gains {amount} Burn.");

        battleManager.relicManager.Trigger(
            RelicTriggerType.OnApplyBurn,
            battleManager.relicManager.CreateContext(source: null, target: target)
        );

        bool hasPoison = target.statusData.Has(StatusEffectType.Poison);
        int burnStack = target.statusData.GetStack(StatusEffectType.Burn);

        bool shouldExplode = hasPoison || burnStack >= 3;

        if (shouldExplode)
        {
            string reason = hasPoison && burnStack >= 3
                ? "Poison + Burn and Burn stack >= 3"
                : hasPoison
                    ? "Poison + Burn"
                    : "Burn stack >= 3";

            Debug.Log($"Synergy triggered: {reason} -> Burn Explosion");

            TriggerBurnExplosion(target);
            battleManager.RefreshUI();
            return true;
        }

        battleManager.RefreshUI();
        return false;
    }

    public void ApplyVulnerable(Unit target, int amount)
    {
        if (target == null || amount <= 0)
            return;

        int finalAmount = amount;

        if (battleManager.HasStartPassive(StartPassiveType.VulnerableCore))
        {
            finalAmount += 1;
            Debug.Log("Start Passive triggered: Vulnerable Core (+1 Vulnerable)");
        }

        target.statusData.AddStack(StatusEffectType.Vulnerable, finalAmount);
        Debug.Log($"{target.unitName} gains {finalAmount} Vulnerable.");

        battleManager.RefreshUI();
    }

    private void TriggerBurnExplosion(Unit target)
    {
        Debug.Log($"{target.unitName}'s Burn explodes!");

        int explosionDamage = burnExplosionDamage * burnExplosionDamageMultiplier;
        target.TakeDamage(explosionDamage);

        target.statusData.Clear(StatusEffectType.Burn);

        if (target.statusData.Has(StatusEffectType.Poison))
        {
            Debug.Log("Synergy result: Poison removed due to explosion");
            target.statusData.Clear(StatusEffectType.Poison);
        }

        if (battleManager.HasStartPassive(StartPassiveType.BurnCore))
        {
            Debug.Log("Start Passive triggered: Reapply Burn 1 after explosion");
            target.statusData.AddStack(StatusEffectType.Burn, 1);
        }

        battleManager.relicManager.Trigger(
            RelicTriggerType.OnBurnExploded,
            battleManager.relicManager.CreateContext(source: null, target: target)
        );
    }

    public void ProcessTurnEnd(Unit unit)
    {
        if (unit == null)
            return;

        ProcessPoison(unit);
        ProcessVulnerable(unit);

        battleManager.RefreshUI();
    }

    private void ProcessPoison(Unit unit)
    {
        int poison = GetStack(unit, StatusEffectType.Poison);

        if (poison <= 0)
            return;

        Debug.Log($"{unit.unitName} takes {poison} Poison damage at turn end.");
        unit.TakeDamage(poison);

        unit.statusData.ReduceStack(StatusEffectType.Poison, 1);
    }

    private void ProcessVulnerable(Unit unit)
    {
        int vulnerable = GetStack(unit, StatusEffectType.Vulnerable);

        if (vulnerable <= 0)
            return;

        unit.statusData.ReduceStack(StatusEffectType.Vulnerable, 1);
        Debug.Log($"{unit.unitName}'s Vulnerable decreases by 1.");
    }

    public bool HasStatus(Unit unit, StatusEffectType type)
    {
        if (unit == null)
            return false;

        return unit.statusData.Has(type);
    }

    public int GetStack(Unit unit, StatusEffectType type)
    {
        if (unit == null)
            return 0;

        return unit.statusData.GetStack(type);
    }

    public int GetDamageWithVulnerable(Unit target, int baseDamage)
    {
        if (target == null)
            return baseDamage;

        bool hasVulnerable = HasStatus(target, StatusEffectType.Vulnerable);
        if (!hasVulnerable)
            return baseDamage;

        bool hasPoison = HasStatus(target, StatusEffectType.Poison);

        float multiplier = hasPoison ? 1.5f : 1.25f;
        int finalDamage = Mathf.RoundToInt(baseDamage * multiplier);

        return finalDamage;
    }
}