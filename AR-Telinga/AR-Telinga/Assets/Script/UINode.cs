using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UINode : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ConnectionManager manager;
    [HideInInspector] public RectTransform rect;

    void Awake() { rect = GetComponent<RectTransform>(); }

    public void OnBeginDrag(PointerEventData e)  { manager.BeginFrom(this); }
    public void OnDrag(PointerEventData e)       { manager.UpdatePointer(e.position); }
    public void OnEndDrag(PointerEventData e)    { manager.EndTo(e.position); }
}
