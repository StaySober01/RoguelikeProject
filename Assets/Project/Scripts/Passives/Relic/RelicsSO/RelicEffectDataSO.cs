using UnityEngine;

public abstract class RelicEffectDataSO : ScriptableObject
{
    public abstract IRelicEffect CreateRuntimeEffect();
}
