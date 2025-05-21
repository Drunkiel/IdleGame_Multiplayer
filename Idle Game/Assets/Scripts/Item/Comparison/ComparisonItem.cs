using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComparisonItem : MonoBehaviour
{
    public ItemID _itemID;

    public Image itemShowcase;
    public TMP_Text itemNameText;
    public TMP_Text holdingTypeText;

    public Transform statContent;
    public TMP_Text statTextPrefab;
    public List<TMP_Text> allContextTexts = new();

    public void OverrideData()
    {
        //ItemData _itemData = null;
        //List<ItemBuff> _itemBuffs = new();
        //Sprite iconSprite = null;

        //switch (_itemID._itemData.itemType)
        //{
        //    case ItemType.Weapon:
        //        _itemData = _itemID._itemData;

        //        holdingTypeText.text = "Holding type:" + _itemID._weaponItem.holdingType;
        //        break;

        //    case ItemType.Armor:
        //        _itemData = _itemID._itemData;

        //        holdingTypeText.text = "Holding type:" + _itemID._armorItem.armorType;
        //        break;
        //}

        //if (_itemData._itemBuffs.Count > 0) 
        //    _itemBuffs = _itemData._itemBuffs;

        ////Removing content
        //for (int i = 0; i < allContextTexts.Count; i++)
        //    Destroy(allContextTexts[i].gameObject);
        //allContextTexts.Clear();

        ////Adding new content
        //for (int i = 0; i < _itemBuffs.Count; i++)
        //{
        //    TMP_Text newStatText = Instantiate(statTextPrefab, statContent);
        //    newStatText.text = $"{_itemBuffs[i].itemBuffs}: {_itemBuffs[i].amount}";
        //    allContextTexts.Add(newStatText);
        //}

        //itemShowcase.sprite = iconSprite;
        //itemNameText.text = _itemData.displayedName;
    }
}
