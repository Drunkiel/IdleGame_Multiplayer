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

    public ItemData(short iD, string displayedName, ItemType itemType, Sprite spriteIcon)
    {
        ID = iD;
        this.displayedName = displayedName;
        this.itemType = itemType;
        this.spriteIcon = spriteIcon;
    }
}
