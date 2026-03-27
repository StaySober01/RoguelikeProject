using UnityEngine;

public class SampleUnit : MonoBehaviour
{
    public string unitName;
    public int maxHp = 30;
    public int currentHp = 30;
    public int attackPower = 5;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp < 0)
            currentHp = 0;

        Debug.Log($"{unitName} takes {damage} damage. Current HP: {currentHp}/{maxHp}");
    }

    public bool IsDead()
    {
        return currentHp <= 0;
    }
}