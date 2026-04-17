using System;
using UnityEngine;

[Serializable]
public class StatusEffectDisplayEntry
{
    public StatusEffectType type;
    public string displayName;
    [TextArea] public string description;
    public Sprite iconSprite;
}
