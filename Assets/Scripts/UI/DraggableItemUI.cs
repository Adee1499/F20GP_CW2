using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItemUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, 
    IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
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
    GameObject _tooltip;
    float _tooltipDelay = 0.5f;
    Coroutine _tooltipCoroutine;
    Vector2 _tooltipOffsetRight;
    Vector2 _tooltipOffsetLeft;
    float _tooltipWidth;
    float _tooltipHeight;
    float _canvasScale;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = FindObjectOfType<Canvas>();
        _canvasScale = _canvas.GetComponent<RectTransform>().localScale.x;
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _draggedItemParent = _canvas.transform.Find("DraggedItemParent");
        _playerReference = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        _tooltip = InventoryUI.Instance.ItemTooltip;
        Rect rect =  _tooltip.GetComponent<RectTransform>().rect;
        _tooltipWidth = rect.width * _canvasScale;
        _tooltipHeight = rect.height * _canvasScale;
        _tooltipOffsetRight = new Vector2(_tooltipWidth / 2 * 1.2f, -(_tooltipHeight / 2 * 1.2f));
        _tooltipOffsetLeft = new Vector2(-(_tooltipWidth / 2 * 1.2f), -(_tooltipHeight / 2 * 1.2f));
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        if (eventData.clickCount > 1) {
            if (!InventoryUI.Instance.UI_Equipment.activeSelf) {
                InventoryUI.Instance.UI_Equipment.SetActive(true);
            }
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tooltipCoroutine = StartCoroutine(ShowTooltip(eventData.position));
        _tooltip.transform.position = eventData.position + (IsTooltipOffscreen(eventData.position) ? _tooltipOffsetLeft : _tooltipOffsetRight);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (_tooltip.activeSelf) {
            Vector2 tooltipPosition = eventData.position + (IsTooltipOffscreen(eventData.position) ? _tooltipOffsetLeft : _tooltipOffsetRight);
            _tooltip.transform.position = tooltipPosition;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipCoroutine != null) {
            StopCoroutine(_tooltipCoroutine);
        }
        _tooltip.SetActive(false);
    }

    IEnumerator ShowTooltip(Vector2 mousePosition)
    {
        yield return new WaitForSeconds(_tooltipDelay);

        if (EventSystem.current.IsPointerOverGameObject()) {
            _tooltip.SetActive(true);
            _tooltip.GetComponent<ItemTooltipUI>().UpdateTooltip(item);
        }
    }

    bool IsTooltipOffscreen(Vector2 mousePosition) {
        return Screen.width - mousePosition.x < _tooltipWidth / 2 + _tooltipOffsetRight.x;
    }
}
