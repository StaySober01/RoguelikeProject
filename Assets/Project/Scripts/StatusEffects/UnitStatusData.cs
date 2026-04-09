using System;
using System.Collections.Generic;

[Serializable]
public class UnitStatusData
{
    private Dictionary<StatusEffectType, int> stacks = new();

    public int GetStack(StatusEffectType type)
    {
        return stacks.TryGetValue(type, out int value) ? value : 0;
    }

    public void AddStack(StatusEffectType type, int amount)
    {
        int newValue = GetStack(type) + amount;

        if (newValue <= 0)
            stacks.Remove(type);
        else
            stacks[type] = newValue;
    }

    public void ReduceStack(StatusEffectType type, int amount)
    {
        int newValue = GetStack(type) - amount;
        if (newValue <= 0)
            stacks.Remove(type);
        else 
            stacks[type] = newValue;
    }

    public void SetStack(StatusEffectType type, int value)
    {
        if (value <= 0)
            stacks.Remove(type);
        else
            stacks[type] = value;
    }

    public bool Has(StatusEffectType type)
    {
        return GetStack(type) > 0;
    }

    public void Clear(StatusEffectType type)
    {
        stacks.Remove(type);
    }

    public void ClearAll()
    {
        stacks.Clear();
    }
}