using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickInventoryController : MonoBehaviour
{
    [SerializeField] private bool lockedUp;
    [SerializeField] private Sprite[] lockStateSprites;
    [SerializeField] private Image lockImage; 
    [SerializeField] private GameObject backgroundImage;
    public List<InventorySlot> _inventorySlots = new();

    private void Start()
    {
        ChangeLockState();
    }

    public bool GetLockedState()
    {
        return lockedUp;
    }

    public void ChangeLockState()
    {
        lockedUp = !lockedUp;
        foreach (InventorySlot _slot in _inventorySlots)
        {
            if (_slot._itemID == null)
            {
                _slot.gameObject.SetActive(!lockedUp);
                backgroundImage.SetActive(!lockedUp);
                continue;
            }

            lockImage.sprite = lockStateSprites[lockedUp ? 0 : 1];
            _slot._itemID.transform.parent.GetComponent<DragDropSlot>().lockedUp = lockedUp;
        }
    }
}
