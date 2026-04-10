using System.Collections.Generic;

public class EffectContext
{
    public BattleManager Battle { get; }
    public Unit Source { get; }
    public Unit Target { get; }
    public CardInstance Card { get; }

    private readonly Dictionary<string, object> tempData = new();

    public EffectContext(BattleManager battle, Unit source, Unit target, CardInstance card)
    {
        Battle = battle;
        Source = source;
        Target = target;
        Card = card;
    }

    public void SetTemp<T>(string key, T value)
    {
        tempData[key] = value;
    }

    public T GetTemp<T>(string key)
    {
        if (tempData.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }

        return default;
    }

    public bool HasTemp(string key)
    {
        return tempData.ContainsKey(key);
    }
}