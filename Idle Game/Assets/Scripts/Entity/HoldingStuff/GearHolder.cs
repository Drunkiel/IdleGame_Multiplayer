using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GearHolder
{
    [Header("Parents of holding Items")]
    public Transform rightHandTransform;
    public Transform leftHandTransform;
    public Transform bothHandTransform;

    public Transform headTransform;
    public Transform bodyTransform;
    public Transform rightFeetTransform;
    public Transform leftFeetTransform;

    [Header("Currently holden Items")]
    public ItemID _weaponItem;
    public ItemID _toolItem1;
    public ItemID _toolItem2;

    public ItemID _armorHead;
    public ItemID _armorChestplate;
    public ItemID _armorBoots;

    [HideInInspector] public bool isFlipped;

    public ItemID GetTool(ToolType toolType)
    {
        //When holding 2 same type tools pick one with better stats
        if (_toolItem1 != null && _toolItem2 != null &&_toolItem1._toolItem != null && _toolItem2._toolItem != null && _toolItem1._toolItem.toolType == _toolItem2._toolItem.toolType)
        {
            if (_toolItem1._toolItem.toolPower >= _toolItem2._toolItem.toolPower)
                return _toolItem1;
            else
                return _toolItem2;
        }

        //When 2 different tools which one correct 
        if (_toolItem1 != null && _toolItem1._toolItem != null && _toolItem1._toolItem.toolType.Equals(toolType))
            return _toolItem1;

        if (_toolItem2 != null && _toolItem2._toolItem != null && _toolItem2._toolItem.toolType.Equals(toolType))
            return _toolItem2;

        return null;
    }

    public ItemID GetHoldingItem(HoldingType holdingType)
    {
        return holdingType switch
        {
            HoldingType.Weapon => _weaponItem,
            HoldingType.Tool_1 => _toolItem1,
            HoldingType.Tool_2 => _toolItem2,
            _ => null,
        };
    }

    public ItemID GetHoldingArmor(HoldingType holdingType)
    {
        return holdingType switch
        {
            HoldingType.Head => _armorHead,
            HoldingType.Chest => _armorChestplate,
            HoldingType.Legs => _armorBoots,
            _ => null,
        };
    }

    public Transform GetHoldingWeaponParent(HoldingType holdingType)
    {
        return holdingType switch
        {
            HoldingType.Weapon => rightHandTransform,
            HoldingType.Tool_1 => leftHandTransform,
            HoldingType.Tool_2 => bothHandTransform,
            _ => null,
        };
    }

    public int GetWeaponDamage()
    {
        int total = 0;

        List<ItemID> equippedItems = new()
        {
            _weaponItem
        };

        foreach (var item in equippedItems)
        {
            if (item == null || item._itemData == null)
                continue;

            //Check if ItemData contains any additional attributes
            if (item._itemData.additionalAttributeStats == null)
                continue;

            //Get weapon damage
            if (item._weaponItem != null && item._itemData.baseStat != null)
                total += item._itemData.baseStat.value;
        }

        return total;
    }

    public EntityAttributes CalculateTotalAttributes()
    {
        EntityAttributes totalAttributes = new();

        // Lista wszystkich ItemID z gearHoldera
        List<ItemID> equippedItems = new()
        {
            _weaponItem,
            _toolItem1,
            _toolItem2,
            _armorHead,
            _armorChestplate,
            _armorBoots
        };

        foreach (var item in equippedItems)
        {
            if (item == null || item._itemData == null)
                continue;

            //Check if ItemData contains any additional attributes
            if (item._itemData.additionalAttributeStats == null)
                continue;

            foreach (var stat in item._itemData.additionalAttributeStats)
            {
                switch (stat.attribute)
                {
                    case Attributes.Strength:
                        totalAttributes.strengthPoints += stat.value;
                        break;
                    case Attributes.Dexterity:
                        totalAttributes.dexterityPoints += stat.value;
                        break;
                    case Attributes.Intelligence:
                        totalAttributes.intelligencePoints += stat.value;
                        break;
                    case Attributes.Durability:
                        totalAttributes.durablityPoints += stat.value;
                        break;
                    case Attributes.Luck:
                        totalAttributes.luckPoints += stat.value;
                        break;
                }
            }

            //Get armorPoints
            if (item._armorItem != null && item._itemData.baseStat != null)
                totalAttributes.armorPoints += item._itemData.baseStat.value;
        }

        return totalAttributes;
    }
}
