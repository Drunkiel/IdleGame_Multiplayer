using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Items/Armor data")]
public class ArmorData : ScriptableObject
{
    public ItemData _itemData;
    public ArmorType armorType;
}
