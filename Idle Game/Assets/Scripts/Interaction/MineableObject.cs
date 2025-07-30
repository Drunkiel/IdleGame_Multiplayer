using UnityEngine;

public class MineableObject : MonoBehaviour
{
    public int hitPoints;
    public ToolType toolType;
    public int minPower;

    public void TakeHit(ItemID _itemID)
    {
        //Check if is using correct tool
        if (_itemID._itemData.itemType.Equals(ItemType.Tool) && _itemID._toolItem.toolType.Equals(toolType))
            return;

        if (_itemID._itemData.baseStat.value >= minPower)
            hitPoints -= _itemID._itemData.baseStat.value / minPower;

        if (hitPoints <= 0)
            print('a');
    }
}
