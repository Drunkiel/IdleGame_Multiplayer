using UnityEngine;

public enum WeaponHoldingType
{
    Right_Hand,
    Left_Hand,
    Both_Hands,
}

public enum WeaponType
{
    Sword,
    Shield,
    Bow,
    Wand,
}

public class WeaponItem : MonoBehaviour
{
    public HeroClass classReserved;
    public WeaponType weaponType;
    public WeaponHoldingType holdingType;
    public bool resizable = true;
}
