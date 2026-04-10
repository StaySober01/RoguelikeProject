using System.Collections.Generic;

public class RelicContext
{
    public BattleManager Battle;
    public Unit Source;
    public Unit Target;
    public RelicTriggerType TriggerType;

    public Dictionary<string, object> Data = new();

    public void Set<T>(string key, T value)
    {
        Data[key] = value;
    }

    public T Get<T>(string key)
    {
        if (Data.TryGetValue(key, out var value))
            return (T)value;

        return default;
    }

    public bool Has(string key)
    {
        return Data.ContainsKey(key);
    }
}