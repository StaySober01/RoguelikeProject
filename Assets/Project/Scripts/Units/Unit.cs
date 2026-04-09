using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int maxHp = 30;
    public int currentHp = 30;
    public int attackPower = 5;
    public int currentBlock = 0;
    public StartPassiveType selectedStartPassive;

    public UnitStatusData statusData = new();

    public List<RelicType> activeRelics = new();

    private void Awake()
    {
        currentHp = maxHp;
        currentBlock = 0;
    }

    public void TakeDamage(int damage)
    {
        int remainingDamage = damage;
        float damageMultiplier = 1f;

        bool hasVulnerable = statusData.Has(StatusEffectType.Vulnerable);
        bool hasPoison = statusData.Has(StatusEffectType.Poison);

        if (hasVulnerable)
        {
            damageMultiplier = hasPoison ? 1.5f : 1.25f;
        }

        remainingDamage = Mathf.CeilToInt(damage * damageMultiplier);

        if (currentBlock > 0)
        {
            int blockedAmount = Mathf.Min(currentBlock, remainingDamage);
            currentBlock -= blockedAmount;
            remainingDamage -= blockedAmount;

            Debug.Log($"{unitName} blocks {blockedAmount} damage. Remaining Block: {currentBlock}");
        }

        if (remainingDamage > 0)
        {
            currentHp -= remainingDamage;
            if (currentHp < 0)
                currentHp = 0;

            Debug.Log($"{unitName} takes {remainingDamage} damage. Current HP: {currentHp}/{maxHp}");
        }
        else
        {
            Debug.Log($"{unitName} takes no HP damage.");
        }
    }

    public void AddBlock(int amount)
    {
        currentBlock += amount;
        Debug.Log($"{unitName} gains {amount} Block. Current Block: {currentBlock}");
    }

    public void ResetBlock()
    {
        currentBlock = 0;
    }

    public bool IsDead()
    {
        return currentHp <= 0;
    }

    public void ClearStatusData()
    {
        statusData.ClearAll();
    }
}