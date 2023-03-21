using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Canvas _canvas;
    RectTransform _rectTransform;
    // Required to block raycasts and set transparency
    CanvasGroup _canvasGroup;
    public InventoryItem item;
    Transform _draggedItemParent;
    Transform _previousParent;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = FindObjectOfType<Canvas>();
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _draggedItemParent = _canvas.transform.Find("DraggedItemParent");
    }

    public void OnPointerDown(PointerEventData eventData) {}

    public void OnBeginDrag(PointerEventData eventData)
    {
        _previousParent = transform.parent;
        InventoryUI.Instance.CurrentItem = gameObject;
        transform.SetParent(_draggedItemParent);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;
        if (transform.parent == _draggedItemParent) {
            transform.SetParent(_previousParent);
            transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}
