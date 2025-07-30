using UnityEngine;

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
    public HoldingType holdingType;
    public bool resizable = true;
}
