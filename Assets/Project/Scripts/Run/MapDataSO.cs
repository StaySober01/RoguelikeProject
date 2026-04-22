using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/Map Data")]
public class MapDataSO : ScriptableObject
{
    public List<MapNodeData> nodes = new();

    public MapNodeData GetNode(string nodeId)
    {
        return nodes.Find(node => node.nodeId == nodeId);
    }

    public MapNodeData GetStartNode()
    {
        return nodes.Find(node => node.nodeType == MapNodeType.Start);
    }

    public MapNodeData GetFirstNode()
    {
        return nodes.Count > 0 ? nodes[0] : null;
    }

    public bool IsFirstNode(string nodeId)
    {
        MapNodeData firstNode = GetFirstNode();
        return firstNode != null && firstNode.nodeId == nodeId;
    }

    public bool IsReachableFromCurrent(string currentNodeId, string targetNodeId)
    {
        if (string.IsNullOrEmpty(targetNodeId))
            return false;

        if (string.IsNullOrEmpty(currentNodeId))
            return IsFirstNode(targetNodeId);

        if (currentNodeId == targetNodeId)
            return false;

        MapNodeData currentNode = GetNode(currentNodeId);
        return currentNode != null && currentNode.connectedNodeIds.Contains(targetNodeId);
    }
}
