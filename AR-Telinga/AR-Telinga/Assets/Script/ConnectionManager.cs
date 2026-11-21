using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [Header("UI Refs")]
    public Camera uiCamera;
    public RectTransform canvasRect;

    [Header("Preview Line")]
    public LineRenderer preview;
    public float snapRadiusScreen = 90f;

    [Header("Connection")]
    public ConnectionLine connectionPrefab;

    [Header("Feedback Icons")]
    public GameObject iconBenar;
    public GameObject iconSalah;

    [System.Serializable]
    public class CorrectPair
    {
        public string sourceName;   // nama node asal, misal "A"
        public string targetName;   // nama node jawaban benar, misal "B"
    }

    [Header("Jawaban Benar (atur di Inspector)")]
    public List<CorrectPair> correctAnswers = new List<CorrectPair>();

    private readonly Dictionary<UINode, ConnectionLine> lineByNode = new();
    private readonly Dictionary<UINode, UINode> partner = new();
    private List<UINode> allNodes = new List<UINode>();

    private UINode startNode;
    private bool drawing;

    void Awake()
    {
        if (!uiCamera) uiCamera = Camera.main;
        if (!canvasRect) canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        if (preview)
        {
            preview.useWorldSpace = true;
            preview.positionCount = 0;
            preview.enabled = false;
        }

        allNodes.AddRange(FindObjectsOfType<UINode>(true));
        foreach (var n in allNodes)
            if (!n.manager) n.manager = this;

        if (iconBenar) iconBenar.SetActive(false);
        if (iconSalah) iconSalah.SetActive(false);
    }

    public void BeginFrom(UINode node)
    {
        startNode = node;
        drawing = true;

        if (preview)
        {
            preview.enabled = true;
            preview.positionCount = 2;
            preview.SetPosition(0, startNode.rect.position);
            preview.SetPosition(1, startNode.rect.position);
        }
    }

    public void UpdatePointer(Vector2 screenPos)
    {
        if (!drawing || !preview) return;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screenPos, uiCamera, out var world))
        {
            preview.SetPosition(0, startNode.rect.position);
            preview.SetPosition(1, world);
        }
    }

    public void EndTo(Vector2 screenPos)
    {
        if (!drawing) return;
        drawing = false;

        UINode best = FindClosestNode(screenPos, exclude: startNode, out float dist);

        if (best != null && dist <= snapRadiusScreen)
        {
            ConnectOneToOne(startNode, best);

            // 🔹 Evaluasi benar/salah berdasarkan daftar dari Inspector
            bool correct = IsCorrectPair(startNode.name, best.name);
            ShowFeedback(correct);
        }

        if (preview)
        {
            preview.enabled = false;
            preview.positionCount = 0;
        }

        startNode = null;
    }

    private bool IsCorrectPair(string from, string to)
    {
        foreach (var pair in correctAnswers)
        {
            if (pair.sourceName == from && pair.targetName == to)
                return true;
        }
        return false;
    }

    private UINode FindNodeByName(string nodeName)
    {
        foreach (var node in allNodes)
        {
            if (node != null && node.name == nodeName)
                return node;
        }

        return null;
    }

    public bool IsQuizCompleted()
    {
        if (correctAnswers == null || correctAnswers.Count == 0)
            return false;

        foreach (var pair in correctAnswers)
        {
            var source = FindNodeByName(pair.sourceName);
            var target = FindNodeByName(pair.targetName);

            if (source == null || target == null)
                return false;

            if (!partner.TryGetValue(source, out var connected) || connected != target)
                return false;
        }

        return true;
    }

    public int GetCorrectConnectionCount()
    {
        int count = 0;

        foreach (var pair in correctAnswers)
        {
            var source = FindNodeByName(pair.sourceName);
            var target = FindNodeByName(pair.targetName);

            if (source == null || target == null)
                continue;

            if (partner.TryGetValue(source, out var connected) && connected == target)
                count++;
        }

        return count;
    }

    public void ClearAllConnections()
    {
        foreach (var kvp in lineByNode)
        {
            if (kvp.Value)
                Destroy(kvp.Value.gameObject);
        }

        lineByNode.Clear();
        partner.Clear();

        startNode = null;
        drawing = false;

        if (preview)
        {
            preview.enabled = false;
            preview.positionCount = 0;
        }

        if (iconBenar) iconBenar.SetActive(false);
        if (iconSalah) iconSalah.SetActive(false);
    }

    private UINode FindClosestNode(Vector2 screenPos, UINode exclude, out float bestDist)
    {
        UINode best = null;
        bestDist = float.MaxValue;

        foreach (var n in allNodes)
        {
            if (n == exclude) continue;
            Vector2 nScreen = RectTransformUtility.WorldToScreenPoint(uiCamera, n.rect.position);
            float d = Vector2.Distance(screenPos, nScreen);
            if (d < bestDist)
            {
                bestDist = d;
                best = n;
            }
        }
        return best;
    }

    private void ConnectOneToOne(UINode a, UINode b)
    {
        RemoveConnection(a);
        RemoveConnection(b);

        var line = Instantiate(connectionPrefab, transform);
        line.Init(a.rect, b.rect, uiCamera);

        lineByNode[a] = line;
        lineByNode[b] = line;
        partner[a] = b;
        partner[b] = a;
    }

    public void RemoveConnection(UINode node)
    {
        if (!partner.ContainsKey(node)) return;

        var other = partner[node];
        partner.Remove(node);
        partner.Remove(other);

        if (lineByNode.TryGetValue(node, out var line))
        {
            lineByNode.Remove(node);
            lineByNode.Remove(other);
            if (line) Destroy(line.gameObject);
        }
    }

    private void ShowFeedback(bool isCorrect)
    {
        if (iconBenar) iconBenar.SetActive(isCorrect);
        if (iconSalah) iconSalah.SetActive(!isCorrect);

        CancelInvoke();
        Invoke(nameof(HideFeedback), 1.5f);
    }

    private void HideFeedback()
    {
        if (iconBenar) iconBenar.SetActive(false);
        if (iconSalah) iconSalah.SetActive(false);
    }
}
