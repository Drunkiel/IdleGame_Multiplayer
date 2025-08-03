using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public InventorySlot currentSlot;
    public bool lockedUp;
    public Image image;
    [SerializeField] private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (lockedUp)
            return;

        InventoryController.instance.isMovingItem = true;

        if (currentSlot._itemID != null && currentSlot.itemRestriction != ItemType.None)
        {
            GearHolder _gearHolder = PlayerController.instance._holdingController._itemController._gearHolder;

            if (currentSlot._itemID._armorItem != null)
            {
                //Find armor piece
                ArmorItem _armorItem = _gearHolder.GetHoldingArmor(currentSlot._itemID._armorItem.holdingType)._armorItem;

                //If not null then destroy
                if (_armorItem != null)
                {
                    //Destroy armor piece
                    Destroy(_armorItem);
                    switch (currentSlot._itemID._armorItem.armorType)
                    {
                        case ArmorType.Helmet:
                            Destroy(_gearHolder.headTransform.GetChild(0).gameObject);
                            break;
                        case ArmorType.Chestplate:
                            Destroy(_gearHolder.bodyTransform.GetChild(2).gameObject);
                            break;
                        case ArmorType.Boots:
                            Destroy(_gearHolder.leftFeetTransform.GetChild(0).gameObject);
                            Destroy(_gearHolder.rightFeetTransform.GetChild(0).gameObject);
                            break;
                    }
                }
            }

            if (currentSlot._itemID._weaponItem != null)
            {
                //Find weapon
                WeaponItem _weaponItem = _gearHolder.GetHoldingItem(currentSlot._itemID._weaponItem.holdingType)._weaponItem;

                //Destroy current holding weapon
                if (_weaponItem != null)
                    Destroy(_weaponItem.gameObject);
            }

            if (currentSlot._itemID._toolItem != null)
            {
                //Find tool
                ToolItem _toolItem = currentSlot.slotID == 1 ? _gearHolder._toolItem1._toolItem : _gearHolder._toolItem2._toolItem;

                //Destroy current holding tool
                if (_toolItem != null)
                    Destroy(_toolItem.gameObject);
            }
        }

        currentSlot._itemID = null;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (lockedUp)
            return;

        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (lockedUp)
            return;

        ItemID _itemID = transform.GetChild(1).GetComponent<ItemID>();

        //Check if pointer is over any UI element
        if (!EventSystem.current.gameObject.TryGetComponent(out InventorySlot _inventorySlot) || _itemID._itemData.itemType != _inventorySlot.itemRestriction)
        {
            GearHolder _gearHolder = PlayerController.instance._holdingController._itemController._gearHolder;

            //Checks if there is any armor equipped
            if (_itemID._armorItem != null && _gearHolder.GetHoldingArmor(transform.GetChild(1).GetComponent<ItemID>()._armorItem.holdingType) == null)
                currentSlot._itemID = transform.GetChild(1).GetComponent<ItemID>();

            //Checks if there is any weapon equipped
            if (_itemID._weaponItem != null && _gearHolder.GetHoldingItem(transform.GetChild(1).GetComponent<ItemID>()._weaponItem.holdingType) == null)
                currentSlot._itemID = transform.GetChild(1).GetComponent<ItemID>();

            //Checks if there is any tool equipped
            if (_itemID._toolItem != null && (_gearHolder.GetHoldingItem(HoldingType.Tool_1) == null || _gearHolder.GetHoldingItem(HoldingType.Tool_2) == null))
                currentSlot._itemID = transform.GetChild(1).GetComponent<ItemID>();

            PlayerController.instance._entityInfo.UpdateStats();
        }

        rectTransform.SetParent(currentSlot.transform);
        rectTransform.localPosition = Vector3.zero;
        currentSlot._itemID = transform.GetChild(1).GetComponent<ItemID>();
        InventoryController.instance.UpdateSlots();
        InventoryController.instance.isMovingItem = false;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (lockedUp)
            return;

        if (currentSlot == null)
            currentSlot = eventData.pointerEnter.transform.parent.parent.GetComponent<InventorySlot>();

        if (eventData.button == PointerEventData.InputButton.Right)
            ComparisonController.instance.MakeComparison(currentSlot._itemID);

        rectTransform.SetParent(InventoryController.instance.slotParent.parent);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (lockedUp)
            return;

        rectTransform.SetParent(currentSlot.transform);
        rectTransform.localPosition = Vector3.zero;
    }
}
