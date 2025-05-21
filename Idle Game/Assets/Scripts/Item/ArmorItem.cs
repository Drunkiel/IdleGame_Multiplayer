using UnityEngine;

public enum ArmorType
{
    Helmet,
    Chestplate,
    Boots,
}

public class ArmorItem : MonoBehaviour
{
    public HeroClass classReserved;
    public ArmorType armorType;
}
