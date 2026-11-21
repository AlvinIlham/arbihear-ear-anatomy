using UnityEngine;
using UnityEngine.UI;

/// Gambar garis UI (quad) antara dua RectTransform, mengikuti Panel & bisa di-mask.
[RequireComponent(typeof(CanvasRenderer))]
public class UILine : MaskableGraphic
{
    public RectTransform startPoint;
    public RectTransform endPoint;
    [Range(1f, 20f)] public float thickness = 4f;

    Canvas _canvas;

    protected override void Awake() {
        base.Awake();
        _canvas = GetComponentInParent<Canvas>();
        raycastTarget = false; // supaya tidak mengganggu klik/drag
    }

    Vector2 WorldToLocal(Vector3 world)
    {
        var cam = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                  ? _canvas.worldCamera
                  : null;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            RectTransformUtility.WorldToScreenPoint(cam, world),
            cam,
            out var local);
        return local;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (startPoint == null || endPoint == null) return;

        Vector2 p1 = WorldToLocal(startPoint.position);
        Vector2 p2 = WorldToLocal(endPoint.position);

        Vector2 dir = (p2 - p1).normalized;
        Vector2 n = new Vector2(-dir.y, dir.x) * (thickness * 0.5f);

        // 4 sudut quad
        vh.AddVert(p1 - n, color, Vector2.zero);
        vh.AddVert(p1 + n, color, Vector2.zero);
        vh.AddVert(p2 + n, color, Vector2.zero);
        vh.AddVert(p2 - n, color, Vector2.zero);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    void LateUpdate()
    {
        // refresh tiap frame agar mengikuti drag
        SetVerticesDirty();
    }
}
