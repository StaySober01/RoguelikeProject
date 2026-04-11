using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Values/Value Provider Data")]
public class ValueProviderDataSO : ScriptableObject
{
    public EffectValueType valueType;
    public string tempKey;

    public int multiplier = 1;
    public int flatBonus = 0;

    public int GetValue(EffectContext context)
    {
        int baseValue = 0;

        switch (valueType)
        {
            case EffectValueType.TargetPoisonStack:
                baseValue = context.Battle.statusEffectController
                    .GetStack(context.Target, StatusEffectType.Poison);
                break;

            case EffectValueType.TargetBurnStack:
                baseValue = context.Battle.statusEffectController
                    .GetStack(context.Target, StatusEffectType.Burn);
                break;

            case EffectValueType.TargetVulnerableStack:
                baseValue = context.Battle.statusEffectController
                    .GetStack(context.Target, StatusEffectType .Vulnerable);
                break;

            case EffectValueType.TempIntValue:
                if (context.tempData.TryGetValue(tempKey, out object value) && value is int intVal)
                    baseValue = intVal;
                break;
        }

        return baseValue * multiplier + flatBonus;
    }
}