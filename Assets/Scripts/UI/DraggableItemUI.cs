using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItemUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Canvas _canvas;
    RectTransform _rectTransform;
    // Required to block raycasts and set transparency
    CanvasGroup _canvasGroup;
    public InventoryItem item;
    public bool equipped;
    Transform _draggedItemParent;
    Transform _previousParent;
    GameObject _playerReference;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = FindObjectOfType<Canvas>();
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _draggedItemParent = _canvas.transform.Find("DraggedItemParent");
        _playerReference = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        if (eventData.clickCount > 1) {
            InventorySlotUI slot = FindObjectsOfType<InventorySlotUI>().Where(slot => slot.equipmentSlot == item.equipmentSlot).FirstOrDefault();
            if (equipped) {
                slot.UnequipItem(gameObject);
            } else {
                slot.EquipItem(gameObject);
            }
        }
    }

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
        if (EventSystem.current.IsPointerOverGameObject()) {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1f;
            if (transform.parent == _draggedItemParent) {
                transform.SetParent(_previousParent);
                transform.localPosition = new Vector3(0, 0, 0);
            }
        } else {
            // Instantiate the item's mesh in the world and remove from inventory
            Instantiate(item.onGroundPrefab, _playerReference.transform.position, Quaternion.identity);
            InventoryUI.Instance.RemoveInventoryItem(item);
            Destroy(gameObject);
        }
    }
}
