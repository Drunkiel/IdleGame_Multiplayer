using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public static ItemContainer instance;

    public List<ItemID> _allItems = new();
    public List<ItemID> _weaponItems = new();
    public List<ItemID> _armorItems = new();
    public List<ItemID> _collectableItems = new();

    void Awake()
    {
        instance = this;
    }

    public ItemID GetItemByName(string itemName)
    {
        for (int i = 0; i < _allItems.Count; i++)
        {
            if(_allItems[i]._itemData.displayedName == itemName)
                return _allItems[i];
        }

        return null;
    }

    public ItemID GetItemByNameAndType(string itemName, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Armor:
                for (int i = 0; i < _armorItems.Count; i++)
                {
                    if(_armorItems[i]._itemData.displayedName == itemName)
                        return _armorItems[i];
                }
                return null;

            case ItemType.Weapon:
                for (int i = 0; i < _weaponItems.Count; i++)
                {
                    if(_weaponItems[i]._itemData.displayedName == itemName)
                        return _weaponItems[i];
                }
                return null;

            case ItemType.Collectable:
                for (int i = 0; i < _collectableItems.Count; i++)
                {
                    if(_collectableItems[i]._itemData.displayedName == itemName)
                        return _collectableItems[i];
                }
                return null;
        }

        return null;
    }
}