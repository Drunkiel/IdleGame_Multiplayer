using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    private readonly int countOfSlots = 24;
    public List<InventorySlot> _inventorySlots = new();
    public Transform slotParent;
    [SerializeField] private InventoryAPI _inventoryAPI;

    public bool isMovingItem;

    private void Start()
    {
        instance = this;
    }

    public void LoadInventory()
    {
        StartCoroutine(_inventoryAPI.GetInventoryCoroutine());
    }

    public void AddToInventory(ItemID _itemID, int slotIndex, bool load = false)
    {
        if (slotIndex == -1)
            return;

        _inventorySlots[slotIndex]._itemID = _itemID;
        GameObject slot = Instantiate(_inventorySlots[slotIndex].itemPlacePrefab, _inventorySlots[slotIndex].transform);
        slot.transform.GetChild(0).GetComponent<Image>().sprite = _itemID.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        _inventorySlots[slotIndex]._itemID.transform.SetParent(slot.transform, false);
        if (!load)
            _inventoryAPI.UpdateInventory(new() { _inventorySlots[slotIndex] });

        if (slotIndex < 6)
        {
            ItemController _itemController = PlayerController.instance._holdingController._itemController;
            switch (_inventorySlots[slotIndex].itemRestriction)
            {
                case ItemType.Weapon:
                    bool isFound = false;
                    for (int i = 0; i < _inventorySlots[slotIndex].holdingTypes.Length; i++)
                    {
                        ItemID _weaponItemID = _itemID;
                        if (_weaponItemID._weaponItem.holdingType == _inventorySlots[slotIndex].holdingTypes[i])
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
                    ItemID _armorItemID = _itemID;
                    if (_armorItemID._armorItem.armorType != _inventorySlots[slotIndex].armorType)
                        return;
                    else
                        _itemController.SetArmor(_armorItemID);
                    break;
            }
        }
        //QuestController.instance.InvokeCollectEvent(_itemID._itemData.ID);
    }

    public void UpdateSlot(int slotID)
    {
        _inventoryAPI.UpdateInventory(new() { _inventorySlots[slotID] });
    }

    public void MoveItemToOtherSlot(int currentSlotID, int newSlotID)
    {
        _inventoryAPI.UpdateInventory(new() { _inventorySlots[currentSlotID], _inventorySlots[newSlotID] });
    }

    public void RemoveItemByID(int itemID)
    {
        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            if (_inventorySlots[i]._itemID == null)
                continue;

            if (_inventorySlots[i]._itemID._itemData.ID == itemID)
            {
                Destroy(_inventorySlots[i]._itemID.transform.parent.gameObject);
                return;
            }
        }

        //ConsoleController.instance.ChatMessage(SenderType.System, $"Item: <color=red>{itemID}</color> is not found in the inventory", OutputType.Error);
    }

    public int GetAvailableSlotIndex(ItemID _itemID)
    {
        for (int i = 0; i < countOfSlots; i++)
        {
            if (_inventorySlots[i]._itemID == null)
            {
                switch (_itemID._itemData.itemType)
                {
                    case ItemType.Weapon:
                        for (int j = 0; j < _inventorySlots[i].holdingTypes.Length; j++)
                        {
                            ItemID _weaponItemID = _itemID;
                            if (_weaponItemID._weaponItem.holdingType == _inventorySlots[i].holdingTypes[j])
                                return i;
                        }
                        break;

                    case ItemType.Armor:
                        if (_inventorySlots[i].armorType == _itemID._armorItem.armorType)
                            return i;
                        break;
                }

                if (_inventorySlots[i].itemRestriction == ItemType.None)
                    return i;
            }
        }

        return -1; 
    }
}
