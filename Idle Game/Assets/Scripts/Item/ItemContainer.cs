using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BaseStats
{
    Damage,
    Protection,
}

[Serializable]
public class BaseStat
{
    public BaseStats baseStats;
    public int value;
}

[Serializable]
public class AdditionalAttributeStats
{
    public Attributes attribute;
    public int value;
}

public class ItemContainer : MonoBehaviour
{
    public static ItemContainer instance;

    public List<ItemID> _allItems = new();
    public List<ItemID> _weaponItems = new();
    public List<ItemID> _armorItems = new();
    public List<ItemID> _collectableItems = new();

    void Awake()
    {
        instance = this;
    }

    public ItemID GetItemByID(int itemID)
    {
        for (int i = 0; i < _allItems.Count; i++)
        {
            if (_allItems[i]._itemData.ID == itemID)
                return _allItems[i];
        }

        return null;
    }

    public ItemID GetItemByName(string itemName)
    {
        for (int i = 0; i < _allItems.Count; i++)
        {
            if (_allItems[i]._itemData.displayedName == itemName)
                return _allItems[i];
        }

        return null;
    }

    public ItemID GetItemByNameAndType(string itemName, ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Armor => _armorItems.FirstOrDefault(item => item._itemData.displayedName == itemName),
            ItemType.Weapon => _weaponItems.FirstOrDefault(item => item._itemData.displayedName == itemName),
            ItemType.Collectable => _collectableItems.FirstOrDefault(item => item._itemData.displayedName == itemName),
            _ => null,
        };
    }

    public ItemData GiveItemStats(ItemData _itemData, EntityInfo _entityInfo)
    {
        ItemData _itemCopy = ScriptableObject.CreateInstance<ItemData>();
        _itemCopy.ID = _itemData.ID;
        _itemCopy.displayedName = _itemData.displayedName;
        _itemCopy.itemType = _itemData.itemType;
        _itemCopy.spriteIcon = _itemData.spriteIcon;
        _itemCopy.additionalAttributeStats = new();

        if (_itemData.itemType == ItemType.Collectable)
            return _itemCopy;

        //Give basic stats
            switch (_itemData.itemType)
            {
                case ItemType.Weapon:
                    _itemCopy.baseStat = new()
                    {
                        baseStats = BaseStats.Damage,
                        value = Mathf.CeilToInt(2 * Mathf.Pow(_entityInfo.currentLevel, 1.2f))
                    };
                    break;

                case ItemType.Armor:
                    _itemCopy.baseStat = new()
                    {
                        baseStats = BaseStats.Protection,
                        value = Mathf.CeilToInt(1 * Mathf.Pow(_entityInfo.currentLevel, 1.2f))
                    };
                    break;
            }

        //Add attribute stats
        int totalPoints = Mathf.CeilToInt(5 * Mathf.Pow(_entityInfo.currentLevel, 1.05f));

        var attributeWeights = GetAttributeWeightsForClass(_entityInfo.heroClass);

        var selectedAttributes = SelectAttributesBasedOnDistribution(attributeWeights);

        var distributedPoints = DistributePointsWithWeights(totalPoints, attributeWeights, selectedAttributes);

        foreach (var kvp in distributedPoints)
        {
            _itemCopy.additionalAttributeStats.Add(new AdditionalAttributeStats
            {
                attribute = kvp.Key,
                value = kvp.Value
            });
        }

        return _itemCopy;
    }

    private Dictionary<Attributes, float> GetAttributeWeightsForClass(HeroClass heroClass)
    {
        //Prioritizing points based on class
        return heroClass switch
        {
            HeroClass.Mage => new()
            {
                { Attributes.Strength, 0.05f },
                { Attributes.Dexterity, 0.05f },
                { Attributes.Intelligence, 0.4f },
                { Attributes.Durability, 0.3f },
                { Attributes.Luck, 0.2f },
            },
            HeroClass.Warrior => new()
            {
                { Attributes.Strength, 0.4f },
                { Attributes.Dexterity, 0.05f },
                { Attributes.Intelligence, 0.05f },
                { Attributes.Durability, 0.3f },
                { Attributes.Luck, 0.2f },
            },
            HeroClass.Scout => new()
            {
                { Attributes.Strength, 0.05f },
                { Attributes.Dexterity, 0.4f },
                { Attributes.Intelligence, 0.05f },
                { Attributes.Durability, 0.2f },
                { Attributes.Luck, 0.3f },
            },
            _ => new()
            {
                { Attributes.Strength, 0.2f },
                { Attributes.Dexterity, 0.2f },
                { Attributes.Intelligence, 0.2f },
                { Attributes.Durability, 0.2f },
                { Attributes.Luck, 0.2f },
            }
        };
    }

    private List<Attributes> SelectAttributesBasedOnDistribution(Dictionary<Attributes, float> weights)
    {
        //Choosing how to distribute points
        float rand = UnityEngine.Random.value;

        //For one attribute | 30%
        if (rand < 0.3f)
        {
            return new List<Attributes> { GetWeightedRandomAttribute(weights) };
        }

        //For 3 main attributes (strenght/dexternity/intelligence, durability, luck) | 50%
        if (rand < 0.8f)
        {
            return weights
                .OrderByDescending(kvp => kvp.Value)
                .Take(3)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        //For all of them | 20%
        return weights.Keys.ToList();
    }

    private Attributes GetWeightedRandomAttribute(Dictionary<Attributes, float> weights)
    {
        float totalWeight = weights.Values.Sum();
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var kvp in weights)
        {
            cumulative += kvp.Value;
            if (randomValue <= cumulative)
                return kvp.Key;
        }

        return weights.Keys.First();
    }

    private Dictionary<Attributes, int> DistributePointsWithWeights(
        int totalPoints,
        Dictionary<Attributes, float> weights,
        List<Attributes> selectedAttributes)
    {
        Dictionary<Attributes, int> result = new();

        if (selectedAttributes.Count <= 3)
        {
            //Divide equally
            int baseValue = totalPoints / selectedAttributes.Count;

            foreach (var attr in selectedAttributes)
            {
                result[attr] = baseValue;
            }
        }
        else
        {
            //Divide by weight
            float totalWeight = selectedAttributes.Sum(attr => weights[attr]);

            foreach (var attr in selectedAttributes)
            {
                int allocated = Mathf.FloorToInt(totalPoints * (weights[attr] / totalWeight));
                result[attr] = allocated;
            }
        }

        return result;
    }
}
