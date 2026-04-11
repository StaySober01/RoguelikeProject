using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Conditions/Condition Data")]
public class ConditionDataSO : ScriptableObject
{
    public EffectConditionType conditionType;
    public string tempKey;

    public bool Evaluate(EffectContext context)
    {
        switch (conditionType)
        {
            case EffectConditionType.TargetHasPoison:
                return context.Battle.statusEffectController.HasStatus(context.Target, StatusEffectType.Poison);

            case EffectConditionType.TargetHasBurn:
                return context.Battle.statusEffectController.HasStatus(context.Target, StatusEffectType.Burn);

            case EffectConditionType.TargetHasVulnerable:
                return context.Battle.statusEffectController.HasStatus(context.Target, StatusEffectType.Vulnerable);

            case EffectConditionType.HandHasNoAttack:
                return !context.Battle.hand.Exists(card => card.Category == CardCategory.Attack);

            case EffectConditionType.TempBoolTrue:
                return context.tempData.ContainsKey(tempKey) &&
                       context.tempData[tempKey] is bool value &&
                       value;

            default:
                return false;
        }
    }
}