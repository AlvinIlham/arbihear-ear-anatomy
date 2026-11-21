using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectionLine : MonoBehaviour
{
    public RectTransform a;
    public RectTransform b;
    public Camera uiCamera;

    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
    }

    public void Init(RectTransform _a, RectTransform _b, Camera cam)
    {
        a = _a; b = _b; uiCamera = cam;
        UpdatePositions();
    }

    void LateUpdate() { UpdatePositions(); }

    void UpdatePositions()
    {
        if (!a || !b) return;
        lr.SetPosition(0, a.position);
        lr.SetPosition(1, b.position);
    }
}
