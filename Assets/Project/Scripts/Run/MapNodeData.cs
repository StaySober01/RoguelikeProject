using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapNodeData
{
    public string nodeId;
    public MapNodeType nodeType;
    public Vector2 uiPosition;
    public string encounterId;
    public List<string> connectedNodeIds = new();
}