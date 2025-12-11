using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private bool dragged = false;

    private Vector2 offset;
    piece_controller p;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        p = GetComponent<piece_controller>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragged = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMousePos
        );

        rectTransform.anchoredPosition = localMousePos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragged = true;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMousePos
        );

        rectTransform.anchoredPosition = localMousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        p.onDragEnd();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!dragged)
        {
            p.onDragEnd();
        }
    }

}
