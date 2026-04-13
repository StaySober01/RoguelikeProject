using UnityEngine;

public class GainEnergyOnFirstTurnRelicEffect : IRelicEffect
{
    private readonly int energyAmount;

    public GainEnergyOnFirstTurnRelicEffect(int energyAmount)
    {
        this.energyAmount = energyAmount;
    }

    public void OnEvent(RelicContext context)
    {
        if (context.TriggerType != RelicTriggerType.OnTurnStart)
            return;

        bool isFirstTurn = context.Get<bool>("isFirstTurn");
        if (!isFirstTurn)
            return;

        Debug.Log($"[Relic] Quick Start triggers: gain {energyAmount} Energy");
        context.Battle.AddBattleLog($"Quick Start grants {energyAmount} Energy");
        context.Battle.GainEnergy(energyAmount);
    }
}
