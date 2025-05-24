using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Items/Item data")]
public class ItemData : ScriptableObject
{
    public short ID;
    public string displayedName;
    public ItemType itemType;
    public Sprite spriteIcon;
    public BaseStat baseStat;
    public List<AdditionalAttributeStats> additionalAttributeStats;

    public ItemData(short ID, string displayedName, ItemType itemType, Sprite spriteIcon)
    {
        this.ID = ID;
        this.displayedName = displayedName;
        this.itemType = itemType;
        this.spriteIcon = spriteIcon;
    }
}
