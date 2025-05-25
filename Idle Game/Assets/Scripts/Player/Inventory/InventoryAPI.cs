using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class ItemDataDTO
{
    public short ID;
    public BaseStat baseStat;
    public List<AdditionalAttributeStats> additionalAttributeStats;
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

    public void GetInventory()
    {
        StartCoroutine(GetInventoryCoroutine());
    }

private IEnumerator GetInventoryCoroutine()
{
    UnityWebRequest www = UnityWebRequest.Get($"{ServerConnector.instance.GetServerUrl()}/inventory/{ServerConnector.instance.playerId}");
    yield return www.SendWebRequest();

    if (www.result == UnityWebRequest.Result.Success)
    {
        string json = www.downloadHandler.text;
        inventory = JsonHelper.FromJson<InventorySlotData>(JsonHelper.FixJsonArray(json));

        Debug.Log($"Inventory loaded. Slots: {inventory.Length}");
        foreach (var slot in inventory)
        {
            Debug.Log($"Slot {slot.slotID}:");

            if (slot.itemID != null && slot.itemID._itemData != null)
            {
                var data = slot.itemID._itemData;
                Debug.Log($"Item ID: {data.ID}, Base Value: {data.baseStat.value}");

                foreach (var attr in data.additionalAttributeStats)
                {
                    Debug.Log($"  - Attribute: {attr.attribute}, Value: {attr.value}");
                }
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
