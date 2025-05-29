using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotID;
    public ItemID _itemID;
    public GameObject itemPlacePrefab;
    [HideInInspector] public ItemType itemRestriction;
    [HideInInspector] public WeaponHoldingType[] holdingTypes;
    [HideInInspector] public ArmorType armorType;

    public void OnDrop(PointerEventData eventData)
    {
        RectTransform rectTransform = eventData.pointerDrag.GetComponent<RectTransform>();
        DragDropSlot _dragDropSlot = rectTransform.GetComponent<DragDropSlot>();

        //Check if slot is locked
        if (_dragDropSlot.lockedUp)
            return;

        //Checks if item is correct type
        if (itemRestriction != ItemType.None)
        {
            ItemID _droppedItemID = eventData.pointerDrag.transform.GetChild(1).GetComponent<ItemID>();
            if (_droppedItemID._itemData == null)
            {
                if (itemRestriction != ItemType.Spell)
                    return;
            }
            else if (_droppedItemID._itemData.itemType != itemRestriction)
                return;
        }

        ItemController _itemController = PlayerController.instance._holdingController._itemController;
        switch (itemRestriction)
        {
            case ItemType.Weapon:
                bool isFound = false;
                for (int i = 0; i < holdingTypes.Length; i++)
                {
                    ItemID _weaponItemID = eventData.pointerDrag.transform.GetChild(1).GetComponent<ItemID>();
                    if (_weaponItemID._weaponItem.holdingType == holdingTypes[i])
                    {
                        isFound = true;

                        _itemController.SetWeapon(_weaponItemID);
                        break;
                    }
                }

                //If weapon is not correct then return
                if (!isFound)
                    return;
                break;

            case ItemType.Armor:
                ItemID _armorItemID = eventData.pointerDrag.transform.GetChild(1).GetComponent<ItemID>();
                if (_armorItemID._armorItem.armorType != armorType)
                    return;
                else
                    _itemController.SetArmor(_armorItemID);
                break;
        }

        int oldSlot = _dragDropSlot.currentSlot.slotID;
        _dragDropSlot.currentSlot = this;
        InventoryController.instance.MoveItemToOtherSlot(oldSlot, slotID);
        rectTransform.SetParent(rectTransform.GetComponent<DragDropSlot>().currentSlot.transform);
        rectTransform.localPosition = Vector3.zero;
    }
}
