using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    public RunState CurrentRun { get; private set; } = new();

    [SerializeField] private MapDataSO mapData;
    [SerializeField] private CardDatabaseSO defaultCardDatabase;
    [SerializeField] private int defaultMaxHp = 30;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewRun(Unit playerUnit, CardDatabaseSO cardDatabase)
    {
        CurrentRun = new RunState
        {
            isRunActive = true,
            currentNodeId = GetInitialCurrentNodeId(),
            maxHp = playerUnit != null ? playerUnit.maxHp : defaultMaxHp
        };

        CurrentRun.currentHp = playerUnit != null ? playerUnit.currentHp : CurrentRun.maxHp;
        CurrentRun.selectedStartPassive = playerUnit != null ? playerUnit.selectedStartPassive : default;
        CurrentRun.hasSelectedStartPassive = false;
        CurrentRun.deck = cardDatabase != null ? cardDatabase.CreateStarterDeck() : new List<CardInstance>();

        SceneManager.LoadScene("MapScene");
    }

    public void EnsureRunStarted(Unit playerUnit = null, CardDatabaseSO cardDatabase = null)
    {
        if (CurrentRun != null && CurrentRun.isRunActive)
            return;

        CardDatabaseSO deckSource = cardDatabase != null ? cardDatabase : defaultCardDatabase;

        CurrentRun = new RunState
        {
            isRunActive = true,
            currentNodeId = GetInitialCurrentNodeId(),
            maxHp = playerUnit != null ? playerUnit.maxHp : defaultMaxHp
        };

        CurrentRun.currentHp = playerUnit != null ? playerUnit.currentHp : CurrentRun.maxHp;
        CurrentRun.selectedStartPassive = playerUnit != null ? playerUnit.selectedStartPassive : default;
        CurrentRun.hasSelectedStartPassive = false;
        CurrentRun.deck = deckSource != null ? deckSource.CreateStarterDeck() : new List<CardInstance>();
    }

    public void EnterNode(MapNodeData node)
    {
        if (node == null)
        {
            Debug.LogError("[Run] Cannot enter a null node.");
            return;
        }

        EnsureRunStarted();

        if (mapData != null && !mapData.IsReachableFromCurrent(CurrentRun.currentNodeId, node.nodeId))
        {
            Debug.LogWarning($"[Run] Node {node.nodeId} is not reachable from {CurrentRun.currentNodeId}.");
            return;
        }

        CurrentRun.currentNodeId = node.nodeId;
        CurrentRun.currentEncounterId = node.encounterId;
        SceneManager.LoadScene("BattleScene");
    }

    public void CompleteBattle(bool victory, int playerHp)
    {
        CurrentRun.currentHp = playerHp;

        if (victory)
        {
            if (!CurrentRun.clearedNodeIds.Contains(CurrentRun.currentNodeId))
                CurrentRun.clearedNodeIds.Add(CurrentRun.currentNodeId);

            SceneManager.LoadScene("MapScene");
        }
        else
        {
            CurrentRun.isRunActive = false;
            Debug.Log("[Run] Run failed. Game over flow is not wired yet.");
        }
    }

    public void SaveBattleProgress(Unit playerUnit, List<CardInstance> deck, List<RelicDataSO> relics)
    {
        if (CurrentRun == null)
            CurrentRun = new RunState();

        if (playerUnit != null)
        {
            CurrentRun.maxHp = playerUnit.maxHp;
            CurrentRun.currentHp = playerUnit.currentHp;
            CurrentRun.selectedStartPassive = playerUnit.selectedStartPassive;
            CurrentRun.hasSelectedStartPassive = true;
        }

        if (deck != null)
            CurrentRun.deck = new List<CardInstance>(deck);

        if (relics != null)
            CurrentRun.relics = new List<RelicDataSO>(relics);
    }

    private string GetInitialCurrentNodeId()
    {
        MapNodeData startNode = mapData != null ? mapData.GetStartNode() : null;
        return startNode != null ? startNode.nodeId : string.Empty;
    }
}
