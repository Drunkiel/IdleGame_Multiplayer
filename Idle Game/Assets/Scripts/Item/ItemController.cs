using UnityEngine;

public enum ItemType
{
    None,
    Weapon,
    Armor,
    Collectable,
    Spell,
}

public class ItemController : MonoBehaviour
{
    public GearHolder _gearHolder;

    public bool PickItem(ItemID _itemID, bool isPlayer = true)
    {
        //If npc then do nothing
        if (!isPlayer || _itemID == null)
            return false;

        InventoryController _inventoryController = InventoryController.instance;

        //Looking for available slot in inventory
        int availableSlot = _inventoryController.GetAvailableSlotIndex(_itemID);
        Debug.Log(availableSlot);
        if (availableSlot == -1)
            return false;

        //Cloning item to founded slot and adding it to inventory
        GameObject itemClone = Instantiate(_itemID.gameObject, _inventoryController._inventorySlots[availableSlot].transform);
        itemClone.GetComponent<ItemID>()._itemData = ItemContainer.instance.GiveItemStats(itemClone.GetComponent<ItemID>()._itemData, PlayerController.instance._entityInfo);
        _inventoryController.AddToInventory(itemClone.GetComponent<ItemID>(), availableSlot);

        //Destroy(_itemID.gameObject);
        return true;
    }

    public bool CanPickWeapon(WeaponHoldingType holdingType)
    {
        switch (holdingType)
        {
            case WeaponHoldingType.Right_Hand:
                if (_gearHolder._weaponRight == null && _gearHolder._weaponBoth == null)
                    return true;
                else
                    return false;

            case WeaponHoldingType.Left_Hand:
                if (_gearHolder._weaponLeft == null && _gearHolder._weaponBoth == null)
                    return true;
                else
                    return false;

            case WeaponHoldingType.Both_Hands:
                if (_gearHolder._weaponBoth == null && _gearHolder._weaponLeft == null && _gearHolder._weaponRight == null)
                    return true;
                else
                    return false;
        }

        return false;
    }

    public ItemID PickWeapon(ItemID _itemID, Quaternion rotation, Transform newParent)
    {
        GameObject weaponCopy = Instantiate(_itemID.gameObject, newParent);
        weaponCopy.name = _itemID.name;
        weaponCopy.transform.SetLocalPositionAndRotation(Vector3.zero, rotation);
        if (_itemID._weaponItem.resizable)
            weaponCopy.transform.localScale = Vector3.one;
        
        if (_itemID._weaponItem.weaponType == WeaponType.Sword)
            weaponCopy.transform.localPosition = new(0.9f, 0, 0);

        return weaponCopy.GetComponent<ItemID>();
    }

    public void SetWeapon(ItemID _itemID)
    {
        switch (_itemID._weaponItem.holdingType)
        {
            //Picking weapon to right hand
            case WeaponHoldingType.Right_Hand:
                _gearHolder._weaponRight = PickWeapon(_itemID, Quaternion.Euler(0, 0, -90), _gearHolder.rightHandTransform);
                break;

            //Picking weapon to left hand
            case WeaponHoldingType.Left_Hand:
                Quaternion rotation = Quaternion.Euler(0, 0, -90);

                if (_itemID._weaponItem.weaponType == WeaponType.Shield)
                    rotation = Quaternion.identity;

                _gearHolder._weaponLeft = PickWeapon(_itemID, rotation, _gearHolder.leftHandTransform);
                _gearHolder._weaponLeft.transform.localScale = new(-1, 1, 1);
                break;

            //Picking weapon to both hands
            case WeaponHoldingType.Both_Hands:
                _gearHolder._weaponBoth = PickWeapon(_itemID, Quaternion.Euler(0, 0, -90), _gearHolder.bothHandTransform);
                break;
        }
    }

    public bool CanPickArmor(ArmorType armorType)
    {
        switch (armorType)
        {
            case ArmorType.Helmet:
                if (_gearHolder._armorHead == null)
                    return true;
                else
                    return false;

            case ArmorType.Chestplate:
                if (_gearHolder._armorChestplate == null)
                    return true;
                else
                    return false;

            case ArmorType.Boots:
                if (_gearHolder._armorBoots == null)
                    return true;
                else
                    return false;
        }

        return false;
    }

    public ItemID PickArmor(ItemID _itemID, Transform newParent)
    {
        GameObject armorCopy = Instantiate(_itemID.gameObject, newParent);
        armorCopy.name = _itemID.name;
        armorCopy.transform.localScale = Vector3.one;
        if (_itemID._armorItem.armorType != ArmorType.Boots)
            armorCopy.transform.localPosition = Vector3.zero;
        else
            armorCopy.transform.localPosition = new(0, -0.17f, 0);

        return armorCopy.GetComponent<ItemID>();
    }

    public void SetArmor(ItemID _itemID)
    {
        switch (_itemID._armorItem.armorType)
        {
            //Picking armor to head
            case ArmorType.Helmet:
                _gearHolder._armorHead = PickArmor(_itemID, _gearHolder.headTransform);
                break;

            //Picking armor to body
            case ArmorType.Chestplate:
                _gearHolder._armorChestplate = PickArmor(_itemID, _gearHolder.bodyTransform);
                break;

            //Picking armor to legs
            case ArmorType.Boots:
                _gearHolder._armorBoots = PickArmor(_itemID, _gearHolder.rightFeetTransform);
                PickArmor(_itemID, _gearHolder.leftFeetTransform);
                break;
        }
    }

    //public void ReplaceItem(ItemID _itemID)
    //{
    //    if (_itemID == null)
    //        return;

    //    ItemID _holdingItemID;
    //    InventoryController _inventoryController = InventoryController.instance;

    //    switch (_itemID._itemData.itemType)
    //    {
    //        case ItemType.Weapon:
    //            //Get current item
    //            WeaponItem _weaponItem = _gearHolder.GetHoldingWeapon(_itemID._weaponItem.holdingType)._weaponItem;

    //            //Checks if item is found
    //            if (_weaponItem == null)
    //                return;

    //            //Assign founded item 
    //            _holdingItemID = _weaponItem.GetComponent<ItemID>();

    //            //Making clone of weapon item and assigning it to stand
    //            Transform weaponClone = PickWeapon(_holdingItemID, Quaternion.identity, _itemID.transform.parent).gameObject.transform;
    //            if (weaponClone.GetComponent<ItemID>()._weaponItem.resizable)
    //                weaponClone.localScale = new(0.25f, 0.25f, 0.25f);
    //            weaponClone.localPosition = Vector2.zero;

    //            //Adding clone to inventory
    //            _inventoryController.DeleteGearInventory((int)_weaponItem.holdingType);
    //            _weaponItem = _gearHolder.GetHoldingWeapon(_itemID._weaponItem.holdingType)._weaponItem;
    //            _inventoryController.AddToGearInventory(_itemID, (int)_weaponItem.holdingType);

    //            Destroy(_holdingItemID.gameObject);

    //            //Picking weapon by player
    //            SetWeapon(_itemID);

    //            //Adding clone to inventory
    //            _weaponItem = _gearHolder.GetHoldingWeapon(_itemID._weaponItem.holdingType)._weaponItem;
    //            _inventoryController.DeleteGearInventory((int)_weaponItem.holdingType);
    //            _inventoryController.AddToGearInventory(_itemID, (int)_weaponItem.holdingType);
    //            break;

    //        case ItemType.Armor:
    //            //Get current item
    //            ArmorItem _armorItem = _gearHolder.GetHoldingArmor(_itemID._armorItem.armorType)._armorItem;

    //            //Checks if item is found
    //            if (_armorItem == null)
    //                return;

    //            //Assign founded item 
    //            _holdingItemID = _armorItem.GetComponent<ItemID>();

    //            //Making clone of armor item and assigning it to stand
    //            Transform armorClone = PickArmor(_holdingItemID, _itemID.transform.parent).gameObject.transform;
    //            armorClone.localScale = new(0.5f, 0.5f, 0.5f);
    //            armorClone.localPosition = Vector2.zero;

    //            //If armor is boots then destroy both
    //            if (_holdingItemID._armorItem.armorType == ArmorType.Boots)
    //                Destroy(_gearHolder.leftFeetTransform.GetChild(1).gameObject);
    //            Destroy(_holdingItemID.gameObject);


    //            //Picking armor by player
    //            SetArmor(_itemID);

    //            //Adding clone to inventory
    //            _armorItem = _gearHolder.GetHoldingArmor(_itemID._armorItem.armorType)._armorItem;
    //            _inventoryController.DeleteGearInventory((int)_armorItem.armorType);
    //            _inventoryController.AddToGearInventory(_itemID, (int)_armorItem.armorType);
    //            break;
    //    }

    //    Destroy(_itemID.gameObject);
    //}
}