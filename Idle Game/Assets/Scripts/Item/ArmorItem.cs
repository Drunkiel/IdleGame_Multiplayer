using UnityEngine;

public enum ArmorType
{
    Helmet,
    Chestplate,
    Boots,
}

public class ArmorItem : MonoBehaviour
{
    public ArmorType armorType;
    public Sprite itemSprite;
    public Sprite iconSprite;
}
