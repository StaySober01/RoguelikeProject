using UnityEngine;

public class StatusEffectController : MonoBehaviour
{
    [Header("Status Effect Values")]
    public int burnExplosionThreshold = 3;
    public int poisonDamagePerStack = 1;
    public int burnExplosionDamage = 8;

    [Header("Passive Toggles")]
    public bool bonusPoisonOnApply = true;
    public bool reapplyBurnAfterExplosion = true;

    public void ApplyPoison(Unit target, int amount)
    {
        if (bonusPoisonOnApply)
        {
            amount += 1;
            Debug.Log("Passive triggered: Poison application +1");
        }

        target.statusData.AddStack(StatusEffectType.Poison, amount);

        Debug.Log($"{target.unitName} gains {amount} Poison. Current Poison: {GetStack(target, StatusEffectType.Poison)}");
    }

    public void ApplyBurn(Unit target, int amount)
    {
        target.statusData.AddStack(StatusEffectType.Burn, amount);
        Debug.Log($"{target.unitName} gains {amount} Burn. Current Burn: {GetStack(target, StatusEffectType.Burn)}");

        if (HasStatus(target, StatusEffectType.Poison))
        {
            Debug.Log("Synergy triggered: Burn applied to poisoned target -> immediate explosion");
            TriggerBurnExplosion(target);
            return;
        }

        if (GetStack(target, StatusEffectType.Burn) >= burnExplosionThreshold)
        {
            TriggerBurnExplosion(target);
        }
    }

    public void ProcessTurnEnd(Unit target)
    {
        ProcessPoisonTurnEnd(target);
    }

    private void ProcessPoisonTurnEnd(Unit target)
    {
        int poison = GetStack(target, StatusEffectType.Poison);

        if (poison <= 0)
            return;

        Debug.Log($"{target.unitName} takes {poison} poison damage at turn end.");
        target.TakeDamage(poison);
    }

    private void TriggerBurnExplosion(Unit target)
    {
        Debug.Log($"{target.unitName}'s Burn explodes!");
        target.TakeDamage(burnExplosionDamage);
        target.statusData.Clear(StatusEffectType.Burn);

        if (target.statusData.Has(StatusEffectType.Poison))
        {
            Debug.Log("Synergy result: Poison removed due to explosion");
            target.statusData.Clear(StatusEffectType.Poison);
        }

        if (reapplyBurnAfterExplosion)
        {
            Debug.Log("Passive triggered: Reapply Burn 1 after explosion");
            target.statusData.AddStack(StatusEffectType.Burn, 1);
        }
    }

    public int GetStack(Unit target, StatusEffectType type)
    {
        return target.statusData.GetStack(type);
    }

    public bool HasStatus(Unit target, StatusEffectType type)
    {
        return target.statusData.Has(type);
    }
}