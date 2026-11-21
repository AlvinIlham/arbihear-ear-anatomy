using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropImageSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public DragImageItem currentItem;
    public float wrongDelay = 0.4f;
    public FeedbackUI feedbackUI;
    public RectTransform snapPoint;

    public void OnDrop(PointerEventData eventData)
    {
        DragImageItem draggedItem = eventData.pointerDrag?.GetComponent<DragImageItem>();
        if (draggedItem == null) return;

        if (currentItem != null) return;

        bool isCorrect = draggedItem.correctSlotIndex == slotIndex;

        Debug.Log("Drop on slot " + slotIndex + " | item correct index = " + draggedItem.correctSlotIndex + " | isCorrect = " + isCorrect);

        if (isCorrect)
        {
            currentItem = draggedItem;
            draggedItem.SnapToSlot(this);
            draggedItem.isLocked = true;

            if (feedbackUI != null)
                feedbackUI.ShowBenar();
        }
        else
        {
            if (feedbackUI != null)
                feedbackUI.ShowSalah();

            StartCoroutine(ReturnWrongItem(draggedItem));
        }
    }

    private IEnumerator ReturnWrongItem(DragImageItem item)
    {
        yield return new WaitForSeconds(wrongDelay);
        item.ReturnToStart();
    }
}