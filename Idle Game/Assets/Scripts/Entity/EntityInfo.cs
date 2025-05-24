using System;
using UnityEngine;

[Serializable]
public class EntityAttributes
{
    public int strengthPoints;
    public int dexterityPoints;
    public int intelligencePoints;
    public int durablityPoints;
    public int luckPoints;
    public int armorPoints;
}

[Serializable]
public class EntityInfo
{
    public HeroClass heroClass;
    public string username;
    public int currentLevel
    {
        get;
        private set;
    }
    public int expPoints
    {
        get;
        private set;
    }
    [SerializeField] private int expToNextLvl;
    public int goldCoins
    {
        get;
        private set;
    }

    [Header("Stats")]
    public int hitPoints;
    public int protection;
    public int damage;
    public int criticalChancePercentage;
    public int dodgePercentage;
    public int damageReductionPercentage;

    [Header("Attributes")]
    public EntityAttributes _attributes;
    public EntityAttributes _baseAttributes
    {
        get;
        private set;
    }
    public EntityAttributes _gearAttributes
    {
        get;
        private set;
    }

    public StatisticsUI _statisticsUI;

    public EntityInfo(HeroClass heroClass, string username, int currentLevel, int expPoints, int goldCoins, int strengthPoints, int dexterityPoints, int intelligencePoints, int durablityPoints, int luckPoints, StatisticsUI _statisticsUI = null)
    {
        this.heroClass = heroClass;
        this.username = username;
        this.currentLevel = currentLevel;
        this.expPoints = expPoints;
        expToNextLvl = Mathf.CeilToInt(100 * Mathf.Pow(currentLevel, 1.5f));
        this.goldCoins = goldCoins;
        _baseAttributes = new()
        {
            strengthPoints = strengthPoints,
            dexterityPoints = dexterityPoints,
            intelligencePoints = intelligencePoints,
            durablityPoints = durablityPoints,
            luckPoints = luckPoints
        };
        this._statisticsUI = _statisticsUI;

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

    public void UpdateStats()
    {
        if (currentLevel <= 0)
            return;

        RecalculateStats();
        int weaponDamage = PlayerController.instance._holdingController._itemController._gearHolder.GetWeaponDamage();

        switch (heroClass)
        {
            case HeroClass.Mage:
                hitPoints = _attributes.durablityPoints * 2 * (currentLevel + 1);
                protection = _attributes.strengthPoints / currentLevel;
                damage = weaponDamage * Mathf.RoundToInt(1.25f * (1 + _attributes.intelligencePoints / 10));
                criticalChancePercentage = Mathf.Clamp(_attributes.luckPoints * 5 / ((currentLevel + 1) * 2), 0, 50);
                dodgePercentage = Mathf.Clamp(_attributes.dexterityPoints / currentLevel, 0, 10);
                damageReductionPercentage = Mathf.Clamp(_attributes.armorPoints / (currentLevel + 1), 0, 10);
                break;

            case HeroClass.Warrior:
                hitPoints = _attributes.durablityPoints * 6 * (currentLevel + 1);
                protection = _attributes.intelligencePoints / currentLevel;
                damage = weaponDamage * Mathf.RoundToInt(0.83f * (1 + _attributes.strengthPoints / 10));
                criticalChancePercentage = Mathf.Clamp(_attributes.luckPoints * 5 / ((currentLevel + 1) * 2), 0, 50);
                dodgePercentage = Mathf.Clamp(Mathf.RoundToInt(_attributes.intelligencePoints / (1.2f * currentLevel)), 0, 25);
                damageReductionPercentage = Mathf.Clamp(_attributes.armorPoints / (currentLevel + 1), 0, 50);
                break;

            case HeroClass.Scout:
                hitPoints = _attributes.durablityPoints * 4 * (currentLevel + 1);
                protection = _attributes.strengthPoints / currentLevel;
                damage = weaponDamage * Mathf.RoundToInt(1 * (1 + _attributes.dexterityPoints / 10));
                criticalChancePercentage = Mathf.Clamp(_attributes.luckPoints * 5 / ((currentLevel + 1) * 2), 0, 50);
                dodgePercentage = Mathf.Clamp(Mathf.RoundToInt(_attributes.intelligencePoints / (1.1f * currentLevel)), 0, 50);
                damageReductionPercentage = Mathf.Clamp(_attributes.armorPoints / (currentLevel + 1), 0, 25);
                break;
        }

        if (_statisticsUI != null)
        {
            //Update just points
            _statisticsUI.strengthPointsText.text = _attributes.strengthPoints.ToString();
            _statisticsUI.dexterityPointsText.text = _attributes.dexterityPoints.ToString();
            _statisticsUI.intelligencePointsText.text = _attributes.intelligencePoints.ToString();
            _statisticsUI.durabilityPointsText.text = _attributes.durablityPoints.ToString();
            _statisticsUI.luckPointsText.text = _attributes.luckPoints.ToString();
            _statisticsUI.armorPointsText.text = _attributes.armorPoints.ToString();

            //Update actual stats
            _statisticsUI.hitPointsText.text = $"Hit points: {hitPoints}";
            _statisticsUI.protectionText.text = $"Protection: {protection}";
            _statisticsUI.damageText.text = $"Damage: ~{damage}";
            _statisticsUI.criticalChanceText.text = $"Critical chance: {criticalChancePercentage}%";
            _statisticsUI.dodgeText.text = $"Dodge chance: {dodgePercentage}%";
            _statisticsUI.damageReductionText.text = $"Damage reduction: {damageReductionPercentage}%";
        }
    }

    private void RecalculateStats()
    {
        _gearAttributes = PlayerController.instance._holdingController._itemController._gearHolder.CalculateTotalAttributes();
        _attributes = new()
        {
            strengthPoints = _baseAttributes.strengthPoints + _gearAttributes.strengthPoints,
            dexterityPoints = _baseAttributes.dexterityPoints + _gearAttributes.dexterityPoints,
            intelligencePoints = _baseAttributes.intelligencePoints + _gearAttributes.intelligencePoints,
            durablityPoints = _baseAttributes.durablityPoints + _gearAttributes.durablityPoints,
            luckPoints = _baseAttributes.luckPoints + _gearAttributes.luckPoints,
            armorPoints = _gearAttributes.armorPoints,
        };
    }

    public void AddPoint(int index)
    {
        Attributes attribute = (Attributes)Enum.Parse(typeof(Attributes), index.ToString());

        switch (attribute)
        {
            case Attributes.Strength:
                _baseAttributes.strengthPoints += 1;
                break;

            case Attributes.Dexterity:
                _baseAttributes.dexterityPoints += 1;
                break;

            case Attributes.Intelligence:
                _baseAttributes.intelligencePoints += 1;
                break;

            case Attributes.Durability:
                _baseAttributes.durablityPoints += 1;
                break;

            case Attributes.Luck:
                _baseAttributes.luckPoints += 1;
                break;
        }

        UpdateStats();
    }
}
