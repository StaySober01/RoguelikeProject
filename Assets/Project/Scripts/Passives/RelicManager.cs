using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public List<RelicType> activeRelics = new();
    private RelicEffectCache relicEffectCache = new();

    private BattleManager battleManager;
    private StatusEffectController statusEffectController;

    public void Initialize(BattleManager battleManager, StatusEffectController statusEffectController)
    {
        this.battleManager = battleManager;
        this.statusEffectController = statusEffectController;
        RebuildRelicEffectCache();
    }

    public void AddRelic(RelicType relicType)
    {
        if (activeRelics.Contains(relicType))
            return;

        activeRelics.Add(relicType);
        RebuildRelicEffectCache();
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

    public void RebuildRelicEffectCache()
    {
        relicEffectCache = new RelicEffectCache();

        foreach (RelicType relic in activeRelics)
        {
            switch (relic)
            {
                case RelicType.VenomSac:
                    relicEffectCache.bonusPoisonOnApply += 1;
                    break;

                case RelicType.SmolderingAsh:
                    relicEffectCache.drawOnBurnExplosion += 1;
                    break;

                case RelicType.VolatileMixture:
                    relicEffectCache.bonusDamageToPoisonAndBurnTarget += 2;
                    break;

                case RelicType.PressurePoint:
                    relicEffectCache.gainEnergyOnAttackVulnerableTarget += 1;
                    break;

                case RelicType.OpeningSalvo:
                    relicEffectCache.damageToEnemyAtTurnStart += 3;
                    break;

                case RelicType.QuickStart:
                    relicEffectCache.bonusEnergyOnFirstTurn += 1;
                    break;
            }
        }
    }

    public int GetBonusPoisonOnApply()
    {
        int amount = relicEffectCache.bonusPoisonOnApply;

        if (battleManager.HasStartPassive(StartPassiveType.PoisonCore))
            amount += 1;

        return amount;
    }

    public int GetDrawOnBurnExplosion()
    {
        return relicEffectCache.drawOnBurnExplosion;
    }

    public int GetBonusDamageToPoisonAndBurnTarget(Unit target)
    {
        bool hasPoison = statusEffectController.HasStatus(target, StatusEffectType.Poison);
        bool hasBurn = statusEffectController.HasStatus(target, StatusEffectType.Burn);

        if (hasPoison && hasBurn)
            return relicEffectCache.bonusDamageToPoisonAndBurnTarget;

        return 0;
    }

    public int GetBonusEnergyOnFirstTurn()
    {
        return relicEffectCache.bonusEnergyOnFirstTurn;
    }

    public int GetDamageToEnemyAtTurnStart()
    {
        return relicEffectCache.damageToEnemyAtTurnStart;
    }

    public int GetGainEnergyOnAttackVulnerableTarget()
    {
        return relicEffectCache.gainEnergyOnAttackVulnerableTarget;
    }
}