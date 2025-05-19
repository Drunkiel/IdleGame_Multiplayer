using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Items/Weapon data")]
public class WeaponData : ScriptableObject
{
    public ItemData _itemData;
    public Vector2 size;
    public WeaponType weaponType;
    public WeaponHoldingType holdingType;
    public bool resizable;
}