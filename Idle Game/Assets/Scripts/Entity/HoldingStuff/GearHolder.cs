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
    public WeaponItem _weaponRight;
    public WeaponItem _weaponLeft;
    public WeaponItem _weaponBoth;

    public ArmorItem _armorHead;
    public ArmorItem _armorChestplate;
    public ArmorItem _armorBoots;

    public WeaponItem GetHoldingWeapon(WeaponHoldingType holdingType)
    {
        return holdingType switch
        {
            WeaponHoldingType.Right_Hand => _weaponRight,
            WeaponHoldingType.Left_Hand => _weaponLeft,
            WeaponHoldingType.Both_Hands => _weaponBoth,
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

    public ArmorItem GetHoldingArmor(ArmorType holdingType)
    {
        return holdingType switch
        {
            ArmorType.Helmet => _armorHead,
            ArmorType.Chestplate => _armorChestplate,
            ArmorType.Boots => _armorBoots,
            _ => null,
        };
    }
}
