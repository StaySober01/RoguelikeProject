using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Relics/Relic Data")]
public class RelicDataSO : ScriptableObject
{
    public RelicType relicType;
    public string relicName;
    public string relicDescription;
    public List<RelicEffectDataSO> effectDataList = new();

    public string DisplayName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(relicName))
                return relicName;

            return name;
        }
    }
}
