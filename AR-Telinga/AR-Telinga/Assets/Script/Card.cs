using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IDragHandler, IDropHandler
{
    public Image outline;

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Highlight(true);
    }

    public void Highlight(bool highlight)
    {
        outline.color = highlight ? Color.green : Color.gray;
    }
}
