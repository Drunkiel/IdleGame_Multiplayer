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
    public AdditionalStats additionalStat;
    public List<AdditionalAttributeStats> additionalAttributeStats;
}
