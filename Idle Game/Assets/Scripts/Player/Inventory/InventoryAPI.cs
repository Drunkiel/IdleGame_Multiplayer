using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    public IEnumerator GetInventoryCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get($"{ServerConnector.instance.GetServerUrl()}/inventory/{ServerConnector.instance.playerId}");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            inventory = JsonHelper.FromJson<InventorySlotData>(JsonHelper.FixJsonArray(json));

            foreach (var slot in inventory)
            {
                if (slot.itemID != null && slot.itemID._itemData != null)
                {
                    var data = slot.itemID._itemData;

                    //Get item
                    ItemID item = ItemContainer.instance.GetItemByID(data.ID);
                    if (item == null)
                        continue;

                    //Duplicate item and modify it
                    ItemID _itemID = Instantiate(item);
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
                    for (int i = 0; i < data.additionalAttributeStats.Count; i++)
                    {
                        _itemID._itemData.additionalAttributeStats.Add(new() 
                        {
                            attribute = (Attributes)data.additionalAttributeStats[i].attribute,
                            value = data.additionalAttributeStats[i].value,
                        });
                    }

                    //Assign the item
                    if (slot.slotID <= 5)
                        InventoryController.instance.AddToGearInventory(_itemID, slot.slotID);
                    else
                        InventoryController.instance.AddToInventory(_itemID, slot.slotID - 6);
                }
                else
                {
                    Debug.Log("Empty slot.");
                }
            }
        }
        else
        {
            Debug.LogError($"Error fetching inventory: {www.error}");
        }
    }

    // POST example (optional)
    public void UpdateInventory(List<InventorySlot> inventory)
    {
        StartCoroutine(UpdateInventoryCoroutine(inventory));
    }

    private IEnumerator UpdateInventoryCoroutine(List<InventorySlot> inventory)
    {
        string json = JsonHelper.ToJson(inventory.ToArray(), true);
        UnityWebRequest www = UnityWebRequest.Put($"{ServerConnector.instance.GetServerUrl()}/inventory/{ServerConnector.instance.playerId}", json);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Inventory updated successfully");
        }
        else
        {
            Debug.LogError($"Error updating inventory: {www.error}");
        }
    }

    // Helper fix for raw JSON arrays
    private string FixJsonArray(string json)
    {
        if (!json.StartsWith("["))
            return json;

        return "{\"Items\":" + json + "}";
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T> { Items = array };
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    public static string FixJsonArray(string json)
    {
        return "{\"Items\":" + json + "}";
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
