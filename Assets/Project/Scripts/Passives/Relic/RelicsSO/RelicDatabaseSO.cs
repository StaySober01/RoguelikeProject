using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Relics/Relic Database")]
public class RelicDatabaseSO : ScriptableObject
{
    [Header("Reward Pool")]
    public List<RelicDataSO> rewardPoolRelics = new();

    public List<RelicDataSO> CreateRewardPool(RelicManager relicManager)
    {
        List<RelicDataSO> rewardPool = new();

        foreach (RelicDataSO relicData in rewardPoolRelics)
        {
            if (relicData == null)
            {
                Debug.LogError("[RelicDatabase] Null relic entry found in reward pool.");
                continue;
            }

            if (relicManager != null && relicManager.HasRelic(relicData.relicType))
                continue;

            rewardPool.Add(relicData);
        }

        return rewardPool;
    }
}
