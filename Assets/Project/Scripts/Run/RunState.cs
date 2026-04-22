using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunState
{
    public string currentNodeId;
    public string currentEncounterId;
    public int currentHp;
    public int maxHp;
    public StartPassiveType selectedStartPassive;
    public bool hasSelectedStartPassive;

    public List<string> clearedNodeIds = new();
    public List<CardInstance> deck = new();
    public List<RelicDataSO> relics = new();

    public bool isRunActive;
}
