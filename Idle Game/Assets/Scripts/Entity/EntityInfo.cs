using System;
using UnityEngine;

[Serializable]
public class EntityInfo
{
    public HeroClass heroClass;
    public string username;
    [SerializeField] private int currentLevel;
    [SerializeField] private int expPoints;
    [SerializeField] private int expToNextLvl;
    [SerializeField] private int goldCoins;

    [Header("Stats")]
    public int hitPoints;
    public int protection;
    public int damage;
    public int criticalChancePercentage;
    public int dodgePercentage;
    public int damageReductionPercentage;

    [Header("Attributes")]
    public int strengthPoints;
    public int dexterityPoints;
    public int intelligencePoints;
    public int durablityPoints;
    public int luckPoints;
    public int armorPoints;

    public StatisticsUI _statisticsUI;

    public EntityInfo(HeroClass heroClass, string username, int currentLevel, int expPoints, int goldCoins, int strengthPoints, int dexterityPoints, int intelligencePoints, int durablityPoints, int luckPoints, StatisticsUI _statisticsUI = null)
    {
        this.heroClass = heroClass;
        this.username = username;
        this.currentLevel = currentLevel;
        this.expPoints = expPoints;
        expToNextLvl = Mathf.CeilToInt(100 * Mathf.Pow(currentLevel, 1.5f));
        this.goldCoins = goldCoins;
        this.strengthPoints = strengthPoints;
        this.dexterityPoints = dexterityPoints;
        this.intelligencePoints = intelligencePoints;
        this.durablityPoints = durablityPoints;
        this.luckPoints = luckPoints;
        this._statisticsUI = _statisticsUI;

        if (_statisticsUI != null)
        {
            _statisticsUI.strengthPointsText.text = strengthPoints.ToString();
            _statisticsUI.dexterityPointsText.text = dexterityPoints.ToString();
            _statisticsUI.intelligencePointsText.text = intelligencePoints.ToString();
            _statisticsUI.durabilityPointsText.text = durablityPoints.ToString();
            _statisticsUI.luckPointsText.text = luckPoints.ToString();
            _statisticsUI.armorPointsText.text = armorPoints.ToString();
        }

        UpdateStats();
    }

    public void GiveEXP(int value)
    {
        expPoints += value;

        if (CheckIfCanLvlUp())
            LvlUp();
    }

    private bool CheckIfCanLvlUp()
    {
        if (expPoints >= expToNextLvl)
            return true;

        return false;
    }

    private void LvlUp()
    {
        currentLevel += 1;
        expPoints -= expToNextLvl;
        expToNextLvl = Mathf.CeilToInt(100 * Mathf.Pow(currentLevel, 1.5f));

        if (CheckIfCanLvlUp())
            LvlUp();
    }

    private void UpdateStats()
    {
        if (currentLevel <= 0)
            return;

        switch (heroClass)
        {
            case HeroClass.Mage:
                hitPoints = durablityPoints * 2 * (currentLevel + 1);
                protection = strengthPoints / currentLevel;
                damage = Mathf.RoundToInt(1.25f * (1 + intelligencePoints / 10));
                criticalChancePercentage = Mathf.Clamp(luckPoints * 5 / ((currentLevel + 1) * 2), 0, 50);
                dodgePercentage = Mathf.Clamp(dexterityPoints / currentLevel, 0, 10);
                damageReductionPercentage = Mathf.Clamp(armorPoints / (currentLevel + 1), 0, 10);
                break;

            case HeroClass.Warrior:
                hitPoints = durablityPoints * 6 * (currentLevel + 1);
                protection = intelligencePoints / currentLevel;
                damage = Mathf.RoundToInt(0.83f * (1 + strengthPoints / 10));
                criticalChancePercentage = Mathf.Clamp(luckPoints * 5 / ((currentLevel + 1) * 2), 0, 50);
                dodgePercentage = Mathf.Clamp(Mathf.RoundToInt(intelligencePoints / (1.2f * currentLevel)), 0, 25);
                damageReductionPercentage = Mathf.Clamp(armorPoints / (currentLevel + 1), 0, 50);
                break;

            case HeroClass.Scout:
                hitPoints = durablityPoints * 4 * (currentLevel + 1);
                protection = strengthPoints / currentLevel;
                damage = Mathf.RoundToInt(1 * (1 + dexterityPoints / 10));
                criticalChancePercentage = Mathf.Clamp(luckPoints * 5 / ((currentLevel + 1) * 2), 0, 50);
                dodgePercentage = Mathf.Clamp(Mathf.RoundToInt(intelligencePoints / (1.1f * currentLevel)), 0, 50);
                damageReductionPercentage = Mathf.Clamp(armorPoints / (currentLevel + 1), 0, 25);
                break;
        }

        if (_statisticsUI != null)
        {
            _statisticsUI.hitPointsText.text = $"Hit points: {hitPoints}";
            _statisticsUI.protectionText.text = $"Protection: {protection}";
            _statisticsUI.damageText.text = $"Damage: ~{damage}";
            _statisticsUI.criticalChanceText.text = $"Critical chance: {criticalChancePercentage}%";
            _statisticsUI.dodgeText.text = $"Dodge chance: {dodgePercentage}%";
            _statisticsUI.damageReductionText.text = $"Damage reduction: {damageReductionPercentage}%";
        }
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

        UpdateStats();
    }
}
