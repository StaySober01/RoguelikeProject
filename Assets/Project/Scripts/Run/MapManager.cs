using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private MapDataSO mapData;
    [SerializeField] private MapNodeView nodePrefab;
    [SerializeField] private Transform nodeParent;

    private void Start()
    {
        BuildMap();
    }

    public void BuildMap()
    {
        if (RunManager.Instance == null)
        {
            Debug.LogError("[Map] RunManager is missing. Add RunManager to the first scene before building the map.");
            return;
        }

        if (mapData == null || nodePrefab == null || nodeParent == null)
        {
            Debug.LogError("[Map] MapManager is missing mapData, nodePrefab, or nodeParent.");
            return;
        }

        ClearExistingNodes();
        RunManager.Instance.EnsureRunStarted();

        var run = RunManager.Instance.CurrentRun;

        foreach (var node in mapData.nodes)
        {
            var view = Instantiate(nodePrefab, nodeParent);
            view.transform.localPosition = node.uiPosition;

            bool isCleared = run.clearedNodeIds.Contains(node.nodeId);
            bool isReachable = IsNodeReachable(node.nodeId);

            view.Init(node, isReachable, isCleared);
        }
    }

    private bool IsNodeReachable(string nodeId)
    {
        var run = RunManager.Instance.CurrentRun;
        return mapData.IsReachableFromCurrent(run.currentNodeId, nodeId);
    }

    private void ClearExistingNodes()
    {
        for (int i = nodeParent.childCount - 1; i >= 0; i--)
        {
            Destroy(nodeParent.GetChild(i).gameObject);
        }
    }
}
