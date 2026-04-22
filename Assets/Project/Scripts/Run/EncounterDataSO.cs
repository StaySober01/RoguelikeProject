using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Encounter Data")]
public class EncounterDataSO : ScriptableObject
{
    public string encounterId;
    public EnemyUnit enemyPrefab;
    public bool isElite;
    public int hpBonus;
    public int attackBonus;
}