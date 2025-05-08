using System;
using UnityEngine;

public class EntityInfo : MonoBehaviour
{
    public HeroClass heroClass;
    public string nickname;
    public int currentLevel;
    public int expPoint;
    public int goldCoins;

    [Header("Attributes")]
    public int strengthPoints;
    public int dexterityPoints;
    public int intelligencePoints;
    public int durablityPoints;
    public int luckPoints;
    public int armorPoints;

    public StatisticsUI _statisticsUI;

    public EntityInfo(int currentLevel, int expPoint, int goldCoins, int strengthPoints, int dexterityPoints, int intelligencePoints, int durablityPoints, int luckPoints, int armorPoints)
    {
        this.currentLevel = currentLevel;
        this.expPoint = expPoint;
        this.goldCoins = goldCoins;
        this.strengthPoints = strengthPoints;
        this.dexterityPoints = dexterityPoints;
        this.intelligencePoints = intelligencePoints;
        this.durablityPoints = durablityPoints;
        this.luckPoints = luckPoints;
        this.armorPoints = armorPoints;
    }

    public void AddPoint(int index)
    {
        Attributes attribute = (Attributes)Enum.Parse(typeof(Attributes), index.ToString());

        switch (attribute)
        {
            case Attributes.Strength:
                strengthPoints += 1;
                _statisticsUI.strengthPointsText.text = strengthPoints.ToString();
                break;

            case Attributes.Dexterity:
                dexterityPoints += 1;
                _statisticsUI.dexterityPointsText.text = dexterityPoints.ToString();
                break;

            case Attributes.Intelligence:
                intelligencePoints += 1;
                _statisticsUI.intelligencePointsText.text = intelligencePoints.ToString();
                break;

            case Attributes.Durability:
                durablityPoints += 1;
                _statisticsUI.durabilityPointsText.text = durablityPoints.ToString();
                break;

            case Attributes.Luck:
                luckPoints += 1;
                _statisticsUI.luckPointsText.text = luckPoints.ToString();
                break;
        }
    }
}
