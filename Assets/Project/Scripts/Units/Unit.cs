using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int maxHp = 30;
    public int currentHp = 30;
    public int attackPower = 5;
    public int currentBlock = 0;

    [Header("Status Effects")]
    public int poisonStack = 0;
    public int burnStack = 0;

    private void Awake()
    {
        currentHp = maxHp;
        currentBlock = 0;
        poisonStack = 0;
        burnStack = 0;
    }

    public void TakeDamage(int damage)
    {
        int remainingDamage = damage;

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

    public void ApplyPoison(int amount)
    {
        poisonStack += amount;
        Debug.Log($"{unitName} gains {amount} Poison. Current Poison: {poisonStack}");
    }

    public void ApplyBurn(int amount)
    {
        burnStack += amount;
        Debug.Log($"{unitName} gains {amount} Burn. Current Burn: {burnStack}");
    }

    public void ClearBurn()
    {
        burnStack = 0;
    }

    public bool HasPoison()
    {
        return poisonStack > 0;
    }

    public bool IsDead()
    {
        return currentHp <= 0;
    }
}