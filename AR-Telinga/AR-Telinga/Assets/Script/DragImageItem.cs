using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DragImageItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int correctSlotIndex;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Transform startParent;
    private Vector2 startAnchoredPos;

    [HideInInspector] public DropImageSlot currentSlot;
    [HideInInspector] public bool isLocked = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        startParent = transform.parent;
        startAnchoredPos = rectTransform.anchoredPosition;

        if (currentSlot != null)
        {
            currentSlot.currentItem = null;
            currentSlot = null;
        }

        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (transform.parent == canvas.transform)
        {
            ReturnToStart();
        }
    }

    public void SnapToSlot(DropImageSlot slot)
    {
        currentSlot = slot;

        Transform targetParent = slot.snapPoint != null ? slot.snapPoint : slot.transform;

        transform.SetParent(targetParent, false);

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
    }

    public void ReturnToStart()
    {
        transform.SetParent(startParent, false);
        rectTransform.anchoredPosition = startAnchoredPos;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
    }
}