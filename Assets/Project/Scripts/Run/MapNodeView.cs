using UnityEngine;
using UnityEngine.UI;

public class MapNodeView : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private Image highlight;

    private MapNodeData nodeData;

    public void Init(MapNodeData data, bool isReachable, bool isCleared)
    {
        nodeData = data;

        button.interactable = isReachable;

        // 상태 표현
        if (isCleared)
        {
            highlight.color = Color.gray;
        }
        else if (isReachable)
        {
            highlight.color = Color.white;
        }
        else
        {
            highlight.color = Color.black;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickNode);
    }

    private void OnClickNode()
    {
        RunManager.Instance.EnterNode(nodeData);
    }
}