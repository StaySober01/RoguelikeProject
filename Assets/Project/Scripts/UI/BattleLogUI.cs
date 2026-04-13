using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleLogUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private int maxLogCount = 10;

    private readonly Queue<string> logs = new();

    private int currentLog = 0;

    public void AddLog(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        currentLog++;

        logs.Enqueue($"{currentLog}. " + message);

        while (logs.Count > maxLogCount)
        {
            logs.Dequeue();
        }

        RefreshLogText();
    }

    public void ClearLogs()
    {
        logs.Clear();
        currentLog = 0;
        RefreshLogText();
    }

    private void RefreshLogText()
    {
        if (logText == null)
            return;

        logText.text = string.Join("\n", logs);
    }
}