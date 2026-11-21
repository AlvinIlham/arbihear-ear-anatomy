using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private LineRenderer line;
    private Transform canvas;
    private Vector3 startPos;
    private GameObject currentLine;
    
    void Start()
    {
        canvas = GameObject.Find("Canvas").transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = transform.position;
        currentLine = new GameObject("Line");
        currentLine.transform.SetParent(canvas);
        line = currentLine.AddComponent<LineRenderer>();

        line.positionCount = 2;
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.white;
        line.endColor = Color.white;

        line.SetPosition(0, startPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos = Input.mousePosition;
        worldPos.z = 0f;
        line.SetPosition(1, worldPos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Cek apakah drop ke target
        GameObject target = eventData.pointerCurrentRaycast.gameObject;
        if (target != null && target.GetComponent<DropTarget>() != null)
        {
            // Kunci garis ke target
            line.SetPosition(1, target.transform.position);
            target.GetComponent<DropTarget>().OnMatched(this);
        }
        else
        {
            Destroy(currentLine);
        }
    }
}
