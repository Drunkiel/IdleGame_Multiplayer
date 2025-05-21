using UnityEngine;

public class ItemID : MonoBehaviour
{
    public ItemData _itemData;
    [HideInInspector] public WeaponItem _weaponItem;
    [HideInInspector] public ArmorItem _armorItem;
    [HideInInspector] public CollectableItem _collectableItem;

    private void Start()
    {
        if (TryGetComponent(out WeaponItem _weaponItem))
            this._weaponItem = _weaponItem;

        if (TryGetComponent(out ArmorItem _armorItem))
            this._armorItem = _armorItem;

        if (TryGetComponent(out CollectableItem _collectableItem))
            this._collectableItem = _collectableItem;
    }
}
