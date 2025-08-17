using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

[Serializable]
public class AdditionalAttributeStatsDTO
{
    public int attribute;
    public int value;
}

[Serializable]
public class ItemDataDTO
{
    public short ID;
    public BaseStat baseStat;
    public List<AdditionalAttributeStatsDTO> additionalAttributeStats;
}

[Serializable]
public class ItemIDDTO
{
    public ItemDataDTO _itemData;
}

[Serializable]
public class InventorySlotData
{
    public int slotID;
    public ItemIDDTO itemID;
}

public class InventoryAPI : MonoBehaviour
{
    public InventorySlotData[] inventory;

    public IEnumerator GetInventoryCoroutine(string id, ItemController _itemController = null)
    {
        yield return StartCoroutine(FetchInventoryData(id));
        InitializeInventory(_itemController);
    }

    private IEnumerator FetchInventoryData(string id)
    {
        UnityWebRequest www = UnityWebRequest.Get($"{ServerConnector.instance.GetServerUrl()}/inventory/{id}");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;

            try
            {
                inventory = JsonConvert.DeserializeObject<InventorySlotData[]>(json);
                for (int i = 0; i < inventory.Length; i++)
                    inventory[i].slotID = i;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Deserialization error: {ex.Message}");
            }
        }
        else
            Debug.LogError($"Error fetching inventory: {www.error}");
    }

    private void InitializeInventory(ItemController _itemController)
    {
        if (inventory == null) 
            return;

        foreach (var slot in inventory)
        {
            var data = slot.itemID?._itemData;
            if (data == null || data.baseStat == null)
            {
                if (data != null)
                {
                    data.ID = -1;
                    data.baseStat = null;
                    data.additionalAttributeStats = null;
                }
                continue;
            }

            ItemID item = ItemContainer.instance.GetItemByID(data.ID);

            if (item == null) 
                continue;

            ItemID _itemID = null;

            if (_itemController == null)
            {
                _itemID = Instantiate(item);
                _itemID._itemData = ScriptableObject.CreateInstance<ItemData>();
                _itemID._itemData.ID = data.ID;
                _itemID._itemData.displayedName = item._itemData.displayedName;
                _itemID._itemData.itemType = item._itemData.itemType;
                _itemID._itemData.spriteIcon = item._itemData.spriteIcon;
                _itemID._itemData.baseStat = new()
                {
                    baseStats = item._itemData.baseStat.baseStats,
                    value = data.baseStat.value
                };
                _itemID._itemData.additionalAttributeStats = new();

                foreach (var stat in data.additionalAttributeStats)
                {
                    _itemID._itemData.additionalAttributeStats.Add(new()
                    {
                        attribute = (Attributes)stat.attribute,
                        value = stat.value
                    });
                }
            }

            InventoryController.instance.AddToInventory(_itemID != null ? _itemID : item, slot.slotID, true, _itemController);
        }

        if (_itemController == null)
            PlayerController.instance._entityInfo.UpdateStats();
    }

    public void UpdateInventory()
    {
        List<InventorySlotData> updatedInventory = new();

        foreach (var slot in InventoryController.instance._inventorySlots)
        {
            InventorySlotData slotData = new()
            {
                slotID = slot.slotID
            };

            //Empty slot
            if (slot._itemID == null)
                slotData.itemID = null;
            else
            {
                var itemData = slot._itemID._itemData;

                slotData.itemID = new ItemIDDTO
                {
                    _itemData = new ItemDataDTO
                    {
                        ID = itemData.ID,
                        baseStat = itemData.baseStat,
                        additionalAttributeStats = new()
                    }
                };

                foreach (var attr in itemData.additionalAttributeStats)
                {
                    slotData.itemID._itemData.additionalAttributeStats.Add(new AdditionalAttributeStatsDTO
                    {
                        attribute = (int)attr.attribute,
                        value = attr.value
                    });
                }
            }

            updatedInventory.Add(slotData);
        }

        StartCoroutine(UpdateInventoryCoroutine(updatedInventory.ToArray()));
    }

    private IEnumerator UpdateInventoryCoroutine(InventorySlotData[] inventorySlotDataList)
    {
        var payload = new Dictionary<string, InventorySlotData[]>
        {
            { "Items", inventorySlotDataList }
        };

        string json = JsonConvert.SerializeObject(payload);
        UnityWebRequest www = UnityWebRequest.Put($"{ServerConnector.instance.GetServerUrl()}/inventory/{ServerConnector.instance.playerId}", json);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError($"Error updating inventory: {www.error}");
    }
}
