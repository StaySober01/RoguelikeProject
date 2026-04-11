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

    public void ApplyPoison(
        Unit target,
        int amount,
        bool triggerRelicEvent = true,
        bool applyPoisonCoreBonus = true)
    {
        if (target == null || amount <= 0)
            return;

        int finalAmount = amount;

        if (applyPoisonCoreBonus && battleManager.HasStartPassive(StartPassiveType.PoisonCore))
        {
            finalAmount += 1;
        }

        target.statusData.AddStack(StatusEffectType.Poison, finalAmount);

        if (triggerRelicEvent)
        {
            battleManager.relicManager.Trigger(
                RelicTriggerType.OnApplyPoison,
                battleManager.relicManager.CreateContext(source: null, target: target)
            );
        }

        battleManager.RefreshUI();
    }

    public bool ApplyBurn(Unit target, int amount)
    {
        if (target == null || amount <= 0)
            return false;

        target.statusData.AddStack(StatusEffectType.Burn, amount);

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

            Debug.Log($"[Status] {target.unitName} gains {amount} Burn -> explosion triggered ({reason})");

            TriggerBurnExplosion(target);
            battleManager.RefreshUI();
            return true;
        }

        Debug.Log($"[Status] {target.unitName} gains {amount} Burn");

        battleManager.RefreshUI();
        return false;
    }

    public void ApplyVulnerable(Unit target, int amount)
    {
        if (target == null || amount <= 0)
            return;

        int finalAmount = amount;
        bool triggeredVulnerableCore = false;

        if (battleManager.HasStartPassive(StartPassiveType.VulnerableCore))
        {
            finalAmount += 1;
            triggeredVulnerableCore = true;
        }

        target.statusData.AddStack(StatusEffectType.Vulnerable, finalAmount);
        Debug.Log(triggeredVulnerableCore
            ? $"[Status] {target.unitName} gains {finalAmount} Vulnerable (Vulnerable Core +1)"
            : $"[Status] {target.unitName} gains {finalAmount} Vulnerable");

        battleManager.RefreshUI();
    }

    private void TriggerBurnExplosion(Unit target)
    {
        int explosionDamage = burnExplosionDamage * burnExplosionDamageMultiplier;
        target.TakeDamage(explosionDamage);

        target.statusData.Clear(StatusEffectType.Burn);
        bool removedPoison = false;

        if (target.statusData.Has(StatusEffectType.Poison))
        {
            target.statusData.Clear(StatusEffectType.Poison);
            removedPoison = true;
        }

        bool reappliedBurn = false;
        if (battleManager.HasStartPassive(StartPassiveType.BurnCore))
        {
            target.statusData.AddStack(StatusEffectType.Burn, 1);
            reappliedBurn = true;
        }

        string explosionDetails = $"[Status] {target.unitName}'s Burn explodes for {explosionDamage} damage";
        if (removedPoison)
            explosionDetails += ", Poison cleared";
        if (reappliedBurn)
            explosionDetails += ", Burn Core reapplied Burn 1";
        Debug.Log(explosionDetails);

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

        Debug.Log($"[Status] {unit.unitName} takes {poison} Poison damage at turn end");
        unit.TakeDamage(poison);

        unit.statusData.ReduceStack(StatusEffectType.Poison, 1);
    }

    private void ProcessVulnerable(Unit unit)
    {
        int vulnerable = GetStack(unit, StatusEffectType.Vulnerable);

        if (vulnerable <= 0)
            return;

        unit.statusData.ReduceStack(StatusEffectType.Vulnerable, 1);
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
