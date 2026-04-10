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
        int blockedAmount = 0;
        
        if (currentBlock > 0)
        {
            blockedAmount = Mathf.Min(currentBlock, remainingDamage);
            currentBlock -= blockedAmount;
            remainingDamage -= blockedAmount;
        }

        if (remainingDamage > 0)
        {
            currentHp -= remainingDamage;
            if (currentHp < 0)
                currentHp = 0;
        }

        Debug.Log(
            $"[Battle] {unitName} takes {damage} incoming damage -> Blocked: {blockedAmount}, HP Damage: {remainingDamage}, HP: {currentHp}/{maxHp}, Block: {currentBlock}");
    }

    public void AddBlock(int amount)
    {
        currentBlock += amount;
        Debug.Log($"[Battle] {unitName} gains {amount} Block (Current Block: {currentBlock})");
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
