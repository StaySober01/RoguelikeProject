using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public List<RelicType> activeRelics = new();

    private List<IRelicEffect> runtimeEffects = new();

    private BattleManager battleManager;
    private StatusEffectController statusEffectController;

    public void Initialize(BattleManager battleManager, StatusEffectController statusEffectController)
    {
        this.battleManager = battleManager;
        this.statusEffectController = statusEffectController;
        RebuildRelicEffects();
    }

    public void AddRelic(RelicType relicType)
    {
        if (activeRelics.Contains(relicType))
            return;

        activeRelics.Add(relicType);
        RebuildRelicEffects();

        Debug.Log($"Relic acquired: {relicType}");
    }

    public void ShuffleRelics(List<RelicType> relics)
    {
        for (int i = 0; i < relics.Count; i++)
        {
            RelicType temp = relics[i];
            int randomIndex = Random.Range(i, relics.Count);
            relics[i] = relics[randomIndex];
            relics[randomIndex] = temp;
        }
    }

    public void RebuildRelicEffects()
    {
        runtimeEffects.Clear();

        foreach (RelicType relic in activeRelics)
        {
            switch (relic)
            {
                case RelicType.VenomSac:
                    runtimeEffects.Add(new BonusPoisonOnApplyRelicEffect(1));
                    break;

                case RelicType.SmolderingAsh:
                    runtimeEffects.Add(new DrawOnBurnExplosionRelicEffect(1));
                    break;

                case RelicType.VolatileMixture:
                    runtimeEffects.Add(new BonusDamageToPoisonBurnTargetRelicEffect(2));
                    break;

                case RelicType.PressurePoint:
                    runtimeEffects.Add(new GainEnergyOnAttackVulnerableTargetRelicEffect(1));
                    break;

                case RelicType.OpeningSalvo:
                    runtimeEffects.Add(new DamageEnemyAtTurnStartRelicEffect(3));
                    break;

                case RelicType.QuickStart:
                    runtimeEffects.Add(new GainEnergyOnFirstTurnRelicEffect(1));
                    break;
            }
        }
    }

    public void Trigger(RelicTriggerType trigger, RelicContext context)
    {
        context.TriggerType = trigger;

        foreach (var effect in runtimeEffects)
        {
            effect.OnEvent(context);
        }
    }

    public RelicContext CreateContext(Unit source = null, Unit target = null)
    {
        return new RelicContext
        {
            Battle = battleManager,
            Source = source,
            Target = target
        };
    }

    public bool HasRelic(RelicType relicType)
    {
        return activeRelics.Contains(relicType);
    }
}