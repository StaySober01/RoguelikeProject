using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Encounter Database")]
public class EncounterDatabaseSO : ScriptableObject
{
    public List<EncounterDataSO> encounters = new();

    public EncounterDataSO GetEncounter(string encounterId)
    {
        if (string.IsNullOrEmpty(encounterId))
            return null;

        return encounters.Find(encounter => encounter != null && encounter.encounterId == encounterId);
    }
}
