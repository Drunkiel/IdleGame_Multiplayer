using UnityEngine;

public class PickInteraction : MonoBehaviour
{
    public ItemID _itemID;

    public void Pick()
    {
        if (!PlayerController.instance._holdingController._itemController.PickItem(_itemID))
            return;
    }
}
