using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public List<RelicDataSO> activeRelics = new();

    private List<IRelicEffect> runtimeEffects = new();

    private BattleManager battleManager;
    private StatusEffectController statusEffectController;

    public void Initialize(BattleManager battleManager, StatusEffectController statusEffectController)
    {
        this.battleManager = battleManager;
        this.statusEffectController = statusEffectController;
        RebuildRelicEffects();
    }

    public void AddRelic(RelicDataSO relicData)
    {
        if (relicData == null)
        {
            Debug.LogError("[Relic] Cannot add null relic data.");
            return;
        }

        if (HasRelic(relicData.relicType))
            return;

        activeRelics.Add(relicData);
        RebuildRelicEffects();

        Debug.Log($"[Relic] Acquired: {relicData.DisplayName}");
    }

    public void ShuffleRelics(List<RelicDataSO> relics)
    {
        for (int i = 0; i < relics.Count; i++)
        {
            RelicDataSO temp = relics[i];
            int randomIndex = Random.Range(i, relics.Count);
            relics[i] = relics[randomIndex];
            relics[randomIndex] = temp;
        }
    }

    public void RebuildRelicEffects()
    {
        runtimeEffects.Clear();

        foreach (RelicDataSO relicData in activeRelics)
        {
            if (relicData == null)
            {
                Debug.LogError("[Relic] Null active relic data found while rebuilding effects.");
                continue;
            }

            foreach (RelicEffectDataSO effectData in relicData.effectDataList)
            {
                if (effectData == null)
                {
                    Debug.LogError($"[Relic] Null effect data in relic {relicData.DisplayName}");
                    continue;
                }

                IRelicEffect runtimeEffect = effectData.CreateRuntimeEffect();

                if (runtimeEffect == null)
                {
                    Debug.LogError($"[Relic] Failed to create runtime effect from {effectData.name}");
                    continue;
                }

                runtimeEffects.Add(runtimeEffect);
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
        foreach (RelicDataSO relicData in activeRelics)
        {
            if (relicData == null)
                continue;

            if (relicData.relicType == relicType)
                return true;
        }

        return false;
    }
}
