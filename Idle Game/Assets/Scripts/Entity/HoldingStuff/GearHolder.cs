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
    public ItemID _weaponRight;
    public ItemID _weaponLeft;
    public ItemID _weaponBoth;

    public ItemID _armorHead;
    public ItemID _armorChestplate;
    public ItemID _armorBoots;

    [HideInInspector] public bool isFlipped;

    public void FlipItems(bool isFlipped)
    {
        this.isFlipped = isFlipped;

        if (_weaponRight != null)
        {
            _weaponRight.transform.parent = isFlipped ? leftHandTransform : rightHandTransform;
            _weaponRight.transform.SetLocalPositionAndRotation(new(0.3f, 0, 0), Quaternion.identity);
            _weaponRight.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = isFlipped ? 1 : 6;
        }

        if (_weaponLeft != null)
        {
            _weaponLeft.transform.parent = isFlipped ? rightHandTransform : leftHandTransform;
            _weaponLeft.transform.SetLocalPositionAndRotation(new(0.3f, 0, 0), Quaternion.identity);
            _weaponLeft.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = isFlipped ? 1 : 6;
        }
    }

    public ItemID GetHoldingWeapon(WeaponHoldingType holdingType)
    {
        return holdingType switch
        {
            WeaponHoldingType.Right_Hand => _weaponRight,
            WeaponHoldingType.Left_Hand => _weaponLeft,
            WeaponHoldingType.Both_Hands => _weaponBoth,
            _ => null,
        };
    }

    public ItemID GetHoldingArmor(ArmorType holdingType)
    {
        return holdingType switch
        {
            ArmorType.Helmet => _armorHead,
            ArmorType.Chestplate => _armorChestplate,
            ArmorType.Boots => _armorBoots,
            _ => null,
        };
    }

    public Transform GetHoldingWeaponParent(WeaponHoldingType holdingType)
    {
        return holdingType switch
        {
            WeaponHoldingType.Right_Hand => rightHandTransform,
            WeaponHoldingType.Left_Hand => leftHandTransform,
            WeaponHoldingType.Both_Hands => bothHandTransform,
            _ => null,
        };
    }

    public int GetWeaponDamage()
    {
        int total = 0;

        List<ItemID> equippedItems = new()
        {
            _weaponRight,
            _weaponLeft,
            _weaponBoth
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
            _weaponRight,
            _weaponLeft,
            _weaponBoth,
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
